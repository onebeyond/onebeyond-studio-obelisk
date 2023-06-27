using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace OneBeyond.Studio.FeaturePermissions;
public class FeaturePermissionUserManager<TAuthUser> : UserManager<TAuthUser>
    where TAuthUser : IdentityUser<Guid>
{
    private readonly ClaimComparer _claimComparer = new ClaimComparer(new ClaimComparer.Options() { IgnoreIssuer = true });
    private readonly SignInManager<TAuthUser> _signInManager;
    public FeaturePermissionUserManager(
        IUserStore<TAuthUser> store, 
        IOptions<IdentityOptions> optionsAccessor, 
        IPasswordHasher<TAuthUser> passwordHasher, 
        IEnumerable<IUserValidator<TAuthUser>> userValidators, 
        IEnumerable<IPasswordValidator<TAuthUser>> passwordValidators, 
        ILookupNormalizer keyNormalizer, 
        IdentityErrorDescriber errors, 
        IServiceProvider services, 
        ILogger<UserManager<TAuthUser>> logger,
        SignInManager<TAuthUser> signInManager) : 
        base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
        _signInManager = signInManager;
    }

    public async Task AddFeaturePermissionClaimsAsync(TAuthUser user, IEnumerable<string> featurePermissions, CancellationToken cancellationToken)
    {
        var claimPermissions = featurePermissions.Select(x => new Claim(ApplicationClaims.ApplicationFeature, x));
        cancellationToken.ThrowIfCancellationRequested();
        await AddFeaturePermissionClaimsAsync(user, claimPermissions, cancellationToken).ConfigureAwait(false);
    }

    public async Task AddFeaturePermissionClaimsAsync(TAuthUser user, IEnumerable<Claim> featurePermissions, CancellationToken cancellationToken)
    {
        var claims = await GetClaimsAsync(user).ConfigureAwait(false);

        var featureClaims = claims.Where(claim => claim.Type == ApplicationClaims.ApplicationFeature);

        var removedClaims = featureClaims.Except(featurePermissions, _claimComparer);

        cancellationToken.ThrowIfCancellationRequested();

        await RemoveClaimsAsync(user, removedClaims).ConfigureAwait(false);

        var addedClaims = featurePermissions.Except(featureClaims, _claimComparer);

        await AddClaimsAsync(user, addedClaims).ConfigureAwait(false);

        if (removedClaims.Any() || addedClaims.Any())
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _signInManager.RefreshSignInAsync(user).ConfigureAwait(false);    
        }
    }
 }
