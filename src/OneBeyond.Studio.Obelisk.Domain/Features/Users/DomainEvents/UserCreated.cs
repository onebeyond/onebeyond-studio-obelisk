using System;
using EnsureThat;
using OneBeyond.Studio.Domain.SharedKernel.DomainEvents;

namespace OneBeyond.Studio.Obelisk.Domain.Features.Users.DomainEvents;

public sealed class UserCreated : DomainEvent
{
    public UserCreated(
        Guid userId,
        Uri? resetPasswordUrl)
        : base(DateTimeOffset.UtcNow)
    {
        EnsureArg.IsNotDefault(userId, nameof(userId));
        //Note, resetPasswordUrl can be null for the very first admin we create manually during the initial system setup

        UserId = userId;
        ResetPasswordUrl = resetPasswordUrl;
    }

    public Guid UserId { get; }
    public Uri? ResetPasswordUrl { get; }
}
