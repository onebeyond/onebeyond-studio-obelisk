using EnsureThat;
using OneBeyond.Studio.Core.Mediator.Commands;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.JwtAuthentication.Commands;

public sealed class RefreshJwtToken : ICommand<JwtToken>
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
