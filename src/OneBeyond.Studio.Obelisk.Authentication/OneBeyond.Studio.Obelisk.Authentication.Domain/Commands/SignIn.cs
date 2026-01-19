using OneBeyond.Studio.Core.Mediator;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;

public abstract record SignIn : IRequest<SignInResult>
{
}
