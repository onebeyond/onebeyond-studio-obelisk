using EnsureThat;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;

public sealed record SignInViaProvider : SignIn
{
    public SignInViaProvider(
        SignInProviderLogin login
        )
    {
        EnsureArg.IsNotDefault(login, nameof(login));

        Login = login;
    }

    public SignInProviderLogin Login { get; }
}
