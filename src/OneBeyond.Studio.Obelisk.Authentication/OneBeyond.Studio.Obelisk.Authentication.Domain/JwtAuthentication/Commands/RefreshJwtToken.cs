using EnsureThat;
using OneBeyond.Studio.Core.Mediator;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.JwtAuthentication.Commands;

public sealed class RefreshJwtToken : IRequest<JwtToken>
{
    public RefreshJwtToken(
        string refreshToken
        )
    {
        EnsureArg.IsNotNullOrWhiteSpace(refreshToken, nameof(refreshToken));

        RefreshToken = refreshToken;
    }

    public string RefreshToken { get; }
}
