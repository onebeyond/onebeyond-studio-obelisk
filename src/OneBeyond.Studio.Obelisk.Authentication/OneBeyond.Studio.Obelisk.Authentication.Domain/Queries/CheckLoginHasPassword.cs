using EnsureThat;
using OneBeyond.Studio.Core.Mediator.Queries;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.Queries;

public sealed class CheckLoginHasPassword : IQuery<bool>
{
    public CheckLoginHasPassword(
        string loginId)
    {
        EnsureArg.IsNotNullOrWhiteSpace(loginId, nameof(loginId));

        LoginId = loginId;
    }

    public string LoginId { get; }
}
