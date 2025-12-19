using EnsureThat;
using OneBeyond.Studio.Core.Mediator.Commands;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Commands;

public sealed class SignInTfa : ICommand<SignInResult>
{
    public SignInTfa(
        string code,
        bool rememberMe,
        bool rememberClient)
    {
        EnsureArg.IsNotNullOrWhiteSpace(code, nameof(code));

        Code = code;
        RememberMe = rememberMe;
        RememberClient = rememberClient;
    }

    public string Code { get; }
    public bool RememberMe { get; }
    public bool RememberClient { get; }
}
