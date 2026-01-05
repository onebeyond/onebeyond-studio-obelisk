using EnsureThat;
using OneBeyond.Studio.Core.Mediator.Commands;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.JwtAuthentication.Commands;
public sealed record class SignOutAllTokens : ICommand
{
    public SignOutAllTokens(string username)
    {
        EnsureArg.IsNotNullOrWhiteSpace(username, nameof(username));
        UserName = username;
    }

    public string UserName { get; }
}
