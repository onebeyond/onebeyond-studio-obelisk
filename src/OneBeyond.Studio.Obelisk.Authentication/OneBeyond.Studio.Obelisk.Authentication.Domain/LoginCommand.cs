using EnsureThat;
using OneBeyond.Studio.Core.Mediator;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain;

public abstract record LoginCommand<TResult> : IRequest<TResult>
{
    protected LoginCommand(string loginId)
    {
        EnsureArg.IsNotNullOrWhiteSpace(loginId, nameof(loginId));

        LoginId = loginId;
    }

    public string LoginId { get; }

}
