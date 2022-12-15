using System;
using EnsureThat;
using MediatR;

namespace OneBeyond.Studio.Obelisk.Domain.Features.Users.Commands;

public sealed record SendResetPasswordEmail : IRequest
{
    public SendResetPasswordEmail(
        string loginId,
        Uri resetPasswordUrl)
    {
        EnsureArg.IsNotNullOrWhiteSpace(loginId, nameof(loginId));
        EnsureArg.IsNotNull(resetPasswordUrl, nameof(resetPasswordUrl));

        LoginId = loginId;
        ResetPasswordUrl = resetPasswordUrl;
    }

    public string LoginId { get; }
    public Uri ResetPasswordUrl { get; }
}
