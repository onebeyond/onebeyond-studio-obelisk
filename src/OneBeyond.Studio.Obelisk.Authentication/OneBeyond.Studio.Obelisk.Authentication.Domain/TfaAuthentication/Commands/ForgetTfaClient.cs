using MediatR;
using OneBeyond.Studio.Obelisk.Authentication.Domain;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Commands;

public sealed class ForgetTfaClient : LoginRequest<Unit>
{
    public ForgetTfaClient(string loginId)
        : base(loginId)
    {
    }
}
