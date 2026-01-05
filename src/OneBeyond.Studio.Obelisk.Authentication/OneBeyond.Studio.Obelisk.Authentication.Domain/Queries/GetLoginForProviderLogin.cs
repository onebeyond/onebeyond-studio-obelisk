using EnsureThat;
using OneBeyond.Studio.Core.Mediator.Queries;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.Queries;

public sealed class GetLoginForProviderLogin : IQuery<string?>
{
    public GetLoginForProviderLogin(
        SignInProviderLogin login
        )
    {
        EnsureArg.IsNotDefault(login, nameof(login));

        Login = login;
    }

    public SignInProviderLogin Login { get; }
}
