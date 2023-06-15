using System;
using EnsureThat;
using MediatR;
using OneBeyond.Studio.Obelisk.Authentication.Domain;

namespace OneBeyond.Studio.Obelisk.Domain.Features.Users.Commands;

public sealed record UpdateUser : IRequest<UpdateUserResult>
{
    public UpdateUser(
        Guid userId,
        string userName,
        string email,
        string? roleId,
        bool isActive)
    {
        EnsureArg.IsNotDefault(userId, nameof(userId));
        EnsureArg.IsNotNullOrWhiteSpace(userName, nameof(userName));
        EnsureArg.IsNotNullOrWhiteSpace(email, nameof(email));

        UserId = userId;
        UserName = userName;
        Email = email;
        RoleId = roleId;
        IsActive = isActive;
    }

    public Guid UserId { get; }
    public string Email { get; }
    public string UserName { get; }
    public string? RoleId { get; }
    public bool IsActive { get; }
}
