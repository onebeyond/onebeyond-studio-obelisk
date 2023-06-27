using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using OneBeyond.Studio.Application.SharedKernel.AmbientContexts;
using OneBeyond.Studio.Application.SharedKernel.Repositories;

namespace OneBeyond.Studio.FeaturePermissions.AmbientContexts;
internal sealed class FeaturePermissionRemoteAmbientContextAccessor<TAuthUser> : IAmbientContextAccessor<AmbientContext>
    where TAuthUser : IdentityUser<Guid>   
{
    private readonly IUserIdAccessor _userIdAccessor;    
    private readonly UserManager<TAuthUser> _userManager;

    public FeaturePermissionRemoteAmbientContextAccessor(
        IUserIdAccessor userIdAccessor,
        UserManager<TAuthUser> userManager)
    {
        _userIdAccessor = userIdAccessor;
        _userManager = userManager;
    }

    public AmbientContext AmbientContext => BuildAmbientContextAsync().GetAwaiter().GetResult();

    private async Task<AmbientContext> BuildAmbientContextAsync() {
        var userId = _userIdAccessor.GetLoginId();

        if (userId is null)
        {
            return new AmbientContext(null);
        }

        var authUser = await _userManager.FindByIdAsync(userId.ToString()).ConfigureAwait(false);

        if (authUser is null)
        {
            return new AmbientContext(null);
        }

        var claims = await _userManager.GetClaimsAsync(authUser).ConfigureAwait(false);

        if (claims is null)
        {
            return new AmbientContext(null);
        }

        var roles = await _userManager.GetRolesAsync(authUser).ConfigureAwait(false);

        var roleList = string.Join(",", roles);

        var featurePermissions = claims
            .Where(claim => claim.Type == ApplicationClaims.ApplicationFeature)
            .Select(claim => claim.Value)
            .ToList();

        var userContext = new FeaturePermissionUserContext(
            authUser.Id.ToString(), "User", roleList, featurePermissions);

        return new AmbientContext(userContext);
    }
}
