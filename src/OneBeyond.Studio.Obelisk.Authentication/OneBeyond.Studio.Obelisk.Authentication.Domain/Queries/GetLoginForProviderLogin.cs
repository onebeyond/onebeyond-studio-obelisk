using EnsureThat;
using MediatR;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.Queries;

public sealed class GetLoginForProviderLogin : IRequest<string?>
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
