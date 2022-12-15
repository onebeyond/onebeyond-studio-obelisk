using MediatR;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;

public abstract record SignIn : IRequest<SignInResult>
{
}
