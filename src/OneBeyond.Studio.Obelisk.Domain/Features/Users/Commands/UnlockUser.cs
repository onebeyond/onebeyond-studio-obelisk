using System;
using EnsureThat;
using OneBeyond.Studio.Core.Mediator;

namespace OneBeyond.Studio.Obelisk.Domain.Features.Users.Commands;

public sealed record UnlockUser : IRequest
{
    public UnlockUser(
        Guid userId)
    {
        EnsureArg.IsNotDefault(userId, nameof(userId));

        UserId = userId;
    }

    public Guid UserId { get; }
}
