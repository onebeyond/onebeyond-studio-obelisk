using EnsureThat;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.JwtAuthentication;

public sealed class JwtToken
{
    public JwtToken(
        string loginId,
        string bearerToken,
        string refreshToken
    )
    {
        EnsureArg.IsNotNullOrWhiteSpace(loginId, nameof(loginId));
        EnsureArg.IsNotNullOrWhiteSpace(bearerToken, nameof(bearerToken));
        EnsureArg.IsNotNullOrWhiteSpace(refreshToken, nameof(refreshToken));

        LoginId = loginId;
        BearerToken = bearerToken;
        RefreshToken = refreshToken;
    }

    public string LoginId { get; private set; }
    public string BearerToken { get; private set; }
    public string RefreshToken { get; private set; }
}
