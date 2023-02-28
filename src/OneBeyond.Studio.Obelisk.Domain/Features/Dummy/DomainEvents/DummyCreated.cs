using System;
using EnsureThat;
using OneBeyond.Studio.Domain.SharedKernel.DomainEvents;

namespace OneBeyond.Studio.Obelisk.Domain.Features.Dummies.DomainEvents;

public sealed class DummyCreated : DomainEvent
{
    public DummyCreated(
        Guid dummyId)
        : base(DateTimeOffset.UtcNow)
    {
        EnsureArg.IsNotDefault(dummyId, nameof(dummyId));

        DummyId = dummyId;
    }

    public Guid DummyId { get; }
}
