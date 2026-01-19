using EnsureThat;
using OneBeyond.Studio.Core.Mediator;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;

public sealed record GenerateResetPasswordTokenByLoginId : IRequest<string>
{
    public GenerateResetPasswordTokenByLoginId(string loginId)
    {
        EnsureArg.IsNotNullOrWhiteSpace(loginId, nameof(loginId));

        LoginId = loginId;
    }

    public string LoginId { get; }
}
