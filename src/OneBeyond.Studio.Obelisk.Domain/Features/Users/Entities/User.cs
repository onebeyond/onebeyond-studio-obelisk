using System;
using EnsureThat;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Commands;

namespace OneBeyond.Studio.Obelisk.Domain.Features.Users.Entities;

public sealed class User : UserBase
{
    public User(
        string loginId,
        string userName,
        string email,
        string? roleId,
        Uri? resetPasswordUrl)
        : base(loginId, userName, email, typeof(User).Name, roleId, resetPasswordUrl)
    {
    }

    private User()
    {
    }

    public static User Apply(CreateUser command)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        return new User(
            command.LoginId,
            command.UserName,
            command.Email,
            command.RoleId,
            command.ResetPasswordUrl);
    }
}
