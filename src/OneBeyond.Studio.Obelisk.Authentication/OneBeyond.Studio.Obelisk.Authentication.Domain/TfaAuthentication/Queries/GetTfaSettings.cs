using EnsureThat;
using OneBeyond.Studio.Core.Mediator.Queries;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Queries;

public sealed record GetTfaSettings : IQuery<LoginTfaSettings>
{
    public GetTfaSettings(string loginId)        
    {
        EnsureArg.IsNotNullOrWhiteSpace(loginId, nameof(loginId));

        LoginId = loginId;
    }

    public string LoginId { get; }
}

