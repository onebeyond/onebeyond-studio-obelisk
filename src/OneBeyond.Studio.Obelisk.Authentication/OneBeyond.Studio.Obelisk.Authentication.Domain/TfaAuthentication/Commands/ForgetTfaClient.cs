using EnsureThat;
using OneBeyond.Studio.Core.Mediator;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Commands;

public sealed record ForgetTfaClient : IRequest
{
    public ForgetTfaClient(string loginId)
    {
        EnsureArg.IsNotNullOrWhiteSpace(loginId, nameof(loginId));

        LoginId = loginId;
    }

    public string LoginId { get; }
}
