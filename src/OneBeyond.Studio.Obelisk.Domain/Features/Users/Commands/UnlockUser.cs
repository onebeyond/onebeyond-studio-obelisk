using System;
using EnsureThat;
using OneBeyond.Studio.Core.Mediator.Commands;

namespace OneBeyond.Studio.Obelisk.Domain.Features.Users.Commands;

public sealed record UnlockUser : ICommand
{
    public UnlockUser(
        Guid userId)
    {
        EnsureArg.IsNotDefault(userId, nameof(userId));

        UserId = userId;
    }

    public Guid UserId { get; }
}
