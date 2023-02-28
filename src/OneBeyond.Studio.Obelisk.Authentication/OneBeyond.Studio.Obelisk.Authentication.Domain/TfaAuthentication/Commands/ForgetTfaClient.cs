using MediatR;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Commands;

public sealed record ForgetTfaClient : LoginRequest<Unit>
{
    public ForgetTfaClient(string loginId)
        : base(loginId)
    {
    }
}
