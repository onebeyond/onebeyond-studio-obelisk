using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using OneBeyond.Studio.Domain.SharedKernel.Repositories;
using OneBeyond.Studio.Obelisk.Authentication.Application.Entities;
using OneBeyond.Studio.Obelisk.Authentication.Application.Services.ApplicationClaims;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Exceptions;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Entities;

namespace OneBeyond.Studio.Obelisk.WebApi.AmbientContexts;

internal sealed class ApplicationClaimsIdentityFactory
    : UserClaimsPrincipalFactory<AuthUser>
    , IApplicationClaimsService
{
    private readonly IRORepository<UserBase, Guid> _userRORepository;

    public ApplicationClaimsIdentityFactory(
        UserManager<AuthUser> userManager,
        IOptions<IdentityOptions> optionsAccessor,
        IRORepository<UserBase, Guid> userRORepository)
        : base(userManager, optionsAccessor)
    {
        EnsureArg.IsNotNull(userRORepository, nameof(userRORepository));

        _userRORepository = userRORepository;
    }

    public async override Task<ClaimsPrincipal> CreateAsync(AuthUser user)
    {
        EnsureArg.IsNotNull(user, nameof(user));

        var principal = await base.CreateAsync(user).ConfigureAwait(false);

        IList<string> userRoles = (await UserManager.GetRolesAsync(user).ConfigureAwait(false)).ToList();

        ((ClaimsIdentity)principal.Identity!).AddClaims(userRoles.Select(s => new Claim(ClaimTypes.Role, s)).ToArray());

        var applicationClaims = await ListApplicationClaimsForUserAsync(user.Id, default);

        ((ClaimsIdentity)principal.Identity!).AddClaims(applicationClaims);

        return principal;
    }

    public async Task<IReadOnlyCollection<Claim>> ListApplicationClaimsForUserAsync(
        string userLoginId,
        CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNullOrWhiteSpace(userLoginId, nameof(userLoginId));

        var applicationUser = await FindUserByLoginIdAsync(userLoginId, cancellationToken).ConfigureAwait(false)
            ?? throw new AuthException($"Unable to find an application user for login id {userLoginId}");

        var claims = new List<Claim>
        {
            new Claim(ApplicationClaims.ApplicationUserId, applicationUser.Id.ToString()),
            new Claim(ApplicationClaims.ApplicationUserType, applicationUser.TypeId),
            new Claim(ApplicationClaims.ApplicationUserRole, applicationUser.RoleId!)
        };

        return claims;
    }

    private async Task<UserBase?> FindUserByLoginIdAsync(
        string loginId,
        CancellationToken cancellationToken)
    {
        return (await _userRORepository
                .ListAsync(
                    (user) => user.LoginId == loginId,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false))
            .SingleOrDefault();
    }
}
