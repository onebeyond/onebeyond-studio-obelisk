using EnsureThat;
using MediatR;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain;

public abstract record LoginRequest<TResult> : IRequest<TResult>
{
    protected LoginRequest(string loginId)
    {
        EnsureArg.IsNotNullOrWhiteSpace(loginId, nameof(loginId));

        LoginId = loginId;
    }

    public string LoginId { get; }

}
