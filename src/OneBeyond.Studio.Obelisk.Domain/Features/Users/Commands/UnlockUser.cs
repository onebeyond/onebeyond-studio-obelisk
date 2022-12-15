using System;
using EnsureThat;
using MediatR;

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
