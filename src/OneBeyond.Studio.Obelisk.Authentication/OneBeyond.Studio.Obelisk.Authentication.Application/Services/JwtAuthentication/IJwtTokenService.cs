using System.Threading;
using System.Threading.Tasks;
using OneBeyond.Studio.Obelisk.Authentication.Application.Entities;
using OneBeyond.Studio.Obelisk.Authentication.Domain.JwtAuthentication;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.Services.JwtAuthentication;

internal interface IJwtTokenService
{
    Task<JwtToken> CreateTokenAsync(
        AuthUser identityUser,
        CancellationToken cancellationToken);

    Task<JwtToken> RefreshTokenAsync(
        AuthUser identityUser,
        string oldRefreshToken,
        CancellationToken cancellationToken);

    Task SignOutAsync(AuthUser identityUser);
}
