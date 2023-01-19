using EnsureThat;
using MediatR;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;

public sealed record ChangePassword : IRequest<ChangePasswordResult>
{
    public ChangePassword(
        string oldPassword,
        string newPassword)
    {
        EnsureArg.IsNotNullOrWhiteSpace(oldPassword, nameof(oldPassword));
        EnsureArg.IsNotNullOrWhiteSpace(newPassword, nameof(newPassword));

        OldPassword = oldPassword;
        NewPassword = newPassword;
    }

    public string? LoginId { get; private set; } = default!;
    public string OldPassword { get; }
    public string NewPassword { get; }

    public ChangePassword AttachLoginId(string loginId)
    {
        LoginId = loginId;
        return this;
    }
}
