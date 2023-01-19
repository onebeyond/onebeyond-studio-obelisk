using System;
using EnsureThat;
using MediatR;

namespace OneBeyond.Studio.Obelisk.Domain.Features.Users.Commands;

public sealed record SendResetPasswordEmailSpa : IRequest
{
    public SendResetPasswordEmailSpa(
        string loginId,
        string resetPasswordUrl)
    {
        EnsureArg.IsNotNullOrWhiteSpace(loginId, nameof(loginId));
        EnsureArg.IsNotNull(resetPasswordUrl, nameof(resetPasswordUrl));

        LoginId = loginId;
        ResetPasswordUrl = resetPasswordUrl;
    }

    public string LoginId { get; }
    public string ResetPasswordUrl { get; }
}
