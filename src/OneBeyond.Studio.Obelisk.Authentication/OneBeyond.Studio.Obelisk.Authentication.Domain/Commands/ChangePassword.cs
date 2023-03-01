using EnsureThat;
using MediatR;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;

public sealed record ChangePassword : IRequest<ChangePasswordResult>
{
    public ChangePassword(
        string loginId,
        string oldPassword,
        string newPassword)
    {
        EnsureArg.IsNotNullOrWhiteSpace(loginId, nameof(loginId));
        EnsureArg.IsNotNullOrWhiteSpace(oldPassword, nameof(oldPassword));
        EnsureArg.IsNotNullOrWhiteSpace(newPassword, nameof(newPassword));

        LoginId = loginId;
        OldPassword = oldPassword;
        NewPassword = newPassword;
    }

    public string LoginId { get; }
    public string OldPassword { get; }
    public string NewPassword { get; }
}
