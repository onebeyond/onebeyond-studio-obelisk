
using OneBeyond.Studio.Core.Mediator.Commands;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;

public abstract record SignIn : ICommand<SignInResult>
{
}
