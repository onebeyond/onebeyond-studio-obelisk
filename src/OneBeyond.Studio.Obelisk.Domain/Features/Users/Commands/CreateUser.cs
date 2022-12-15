using System;
using EnsureThat;
using MediatR;

namespace OneBeyond.Studio.Obelisk.Domain.Features.Users.Commands;

public sealed record CreateUser : IRequest<Guid>
{
    public CreateUser(
        string loginId,
        string userName,
        string email,
        string? roleId,
        Uri? resetPasswordUrl)
    {
        EnsureArg.IsNotNullOrWhiteSpace(loginId, nameof(loginId));
        EnsureArg.IsNotNullOrWhiteSpace(userName, nameof(userName));
        EnsureArg.IsNotNullOrWhiteSpace(email, nameof(email));

        LoginId = loginId;
        UserName = userName;
        Email = email;
        RoleId = roleId;
        ResetPasswordUrl = resetPasswordUrl;
    }

    public string LoginId { get; }
    public string Email { get; }
    public string UserName { get; }
    public string? RoleId { get; }
    public Uri? ResetPasswordUrl { get; }
}
