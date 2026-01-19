using EnsureThat;
using OneBeyond.Studio.Core.Mediator;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Queries;

public sealed record GetTfaSettings : IRequest<LoginTfaSettings>
{
    public GetTfaSettings(string loginId)        
    {
        EnsureArg.IsNotNullOrWhiteSpace(loginId, nameof(loginId));

        LoginId = loginId;
    }

    public string LoginId { get; }
}

