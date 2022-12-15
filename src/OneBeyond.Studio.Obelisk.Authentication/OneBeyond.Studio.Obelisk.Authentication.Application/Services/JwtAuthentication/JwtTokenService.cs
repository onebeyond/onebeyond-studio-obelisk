using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Identity;
using OneBeyond.Studio.Obelisk.Authentication.Application.Entities;
using OneBeyond.Studio.Obelisk.Authentication.Application.JwtAuthentication;
using OneBeyond.Studio.Obelisk.Authentication.Application.Services.ApplicationClaims;
using OneBeyond.Studio.Obelisk.Authentication.Domain.JwtAuthentication;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.Services.JwtAuthentication;

internal sealed class JwtTokenService : IJwtTokenService
{
    private readonly UserManager<AuthUser> _userManager;
    private readonly JwtAuthenticationOptions _jwtConfiguration;
    private readonly IApplicationClaimsService _applicationClaimsService;

    public JwtTokenService(
        UserManager<AuthUser> userManager,
        JwtAuthenticationOptions jwtConfiguration,
        IApplicationClaimsService applicationClaimsService
        )
    {
        EnsureArg.IsNotNull(userManager, nameof(userManager));
        EnsureArg.IsNotNull(jwtConfiguration, nameof(jwtConfiguration));
        EnsureArg.IsNotNull(applicationClaimsService, nameof(applicationClaimsService));

        _userManager = userManager;
        _jwtConfiguration = jwtConfiguration;
        _applicationClaimsService = applicationClaimsService;
    }

    public Task<JwtToken> RefreshTokenAsync(
        AuthUser identityUser,
        string oldRefreshToken,
        CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(identityUser, nameof(identityUser));
        EnsureArg.IsNotNullOrWhiteSpace(oldRefreshToken, nameof(oldRefreshToken));

        identityUser.ExpireToken(oldRefreshToken);

        return CreateTokenAsync(
                identityUser,
                cancellationToken);
    }

    public async Task<JwtToken> CreateTokenAsync(
        AuthUser identityUser,
        CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(identityUser, nameof(identityUser));

        var userRoles = (await _userManager.GetRolesAsync(identityUser).ConfigureAwait(false))
            .Select(role => new Claim(ClaimTypes.Role, role))
            .ToList();

        var applicationClaims = await _applicationClaimsService.ListApplicationClaimsForUserAsync(
                identityUser.Id,
                cancellationToken)
            .ConfigureAwait(false);

        var additionalClaims = userRoles.Concat(applicationClaims);

        var jwtToken = TokenGenerator.GenerateJwtToken(
                identityUser,
                additionalClaims,
                _jwtConfiguration);

        var (refreshToken, expiresOn) = TokenGenerator.GenerateRefreshToken(_jwtConfiguration);

        identityUser.AddAuthToken(refreshToken, expiresOn);

        await _userManager.UpdateAsync(identityUser).ConfigureAwait(false); //Save auth token

        return new JwtToken(identityUser.Id, jwtToken, refreshToken);
    }
}
