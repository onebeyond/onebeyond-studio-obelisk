using System.Threading;
using System.Threading.Tasks;
using OneBeyond.Studio.Obelisk.Authentication.Application.Entities;
using OneBeyond.Studio.Obelisk.Authentication.Domain.JwtAuthentication;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.Services.JwtAuthentication;

public interface IJwtTokenService
{
    /// <summary>
    /// Creates a new JWT
    /// </summary>
    /// <param name="identityUser">The user for whom to create a JWT</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<JwtToken> CreateTokenAsync(
        AuthUser identityUser,
        CancellationToken cancellationToken);

    /// <summary>
    /// Refreshes a JWT
    /// </summary>
    /// <param name="identityUser">The user whose token is refreshed</param>
    /// <param name="oldRefreshToken">The previous auth token to expire</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<JwtToken> RefreshTokenAsync(
        AuthUser identityUser,
        string oldRefreshToken,
        CancellationToken cancellationToken);

    /// <summary>
    /// Signs out all JWT for one user.
    /// </summary>
    /// <param name="identityUser">The user to sign out.</param>
    /// <returns></returns>
    Task SignOutAsync(AuthUser identityUser);

    /// <summary>
    /// Clears down tokens expired over a day ago. Prevents excessive table growth.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task CleardownExpiredTokensAsync(CancellationToken cancellationToken);
}
