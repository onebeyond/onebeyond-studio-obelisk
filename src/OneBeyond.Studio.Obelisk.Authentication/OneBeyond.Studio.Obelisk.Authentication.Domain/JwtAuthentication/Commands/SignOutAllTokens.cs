using EnsureThat;
using MediatR;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.JwtAuthentication.Commands;
public sealed record class SignOutAllTokens : IRequest
{
    public SignOutAllTokens(string username)
    {
        EnsureArg.IsNotNullOrWhiteSpace(username, nameof(username));
        UserName = username;
    }

    public string UserName { get; }
}
