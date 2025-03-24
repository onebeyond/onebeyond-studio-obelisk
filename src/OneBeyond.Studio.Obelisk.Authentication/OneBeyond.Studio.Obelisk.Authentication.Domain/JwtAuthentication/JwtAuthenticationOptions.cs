using OneBeyond.Studio.Obelisk.Authentication.Domain.JwtAuthentication.Exceptions;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.JwtAuthentication;

public sealed record JwtAuthenticationOptions
{
    public string? Secret { get; init; }
    public string? Issuer { get; init; }
    public int? AccessTokenExpirationMinutes { get; init; }
    public int? RefreshTokenExpirationDays { get; init; }
    public bool? EnableReplayDefence { get; init; }

    public void EnsureIsValid()
    {
        if (string.IsNullOrEmpty(Secret) || string.IsNullOrEmpty(Issuer))
        {
            throw new JwtAuthenticationFailedException("JWT authentication options are not properly configured.");
        }
    }
}
