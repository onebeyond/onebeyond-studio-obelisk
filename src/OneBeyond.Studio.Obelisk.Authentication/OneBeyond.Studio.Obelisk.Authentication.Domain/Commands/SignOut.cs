using EnsureThat;
using MediatR;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;

public sealed record SignOut : IRequest
{
    public SignOut(
        string loginId)
    {
        EnsureArg.IsNotNullOrWhiteSpace(loginId, nameof(loginId));

        LoginId = loginId;
    }

    public string LoginId { get; }
}
