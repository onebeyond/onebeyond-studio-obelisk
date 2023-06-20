using System;
using OneBeyond.Studio.ChangeTracker.Domain.Attributes;
using OneBeyond.Studio.Domain.SharedKernel.Entities;

namespace OneBeyond.Studio.Obelisk.Domain.Features.Audit.Entities;

[ChangeTrackerIgnore]
public sealed class AuditRecord: AggregateRoot<int>
{
    public AuditRecord(
        DateTimeOffset date,
        string userId,
        string entityId,
        string entityType,
        string changeType,
        string changes,
        string children)
    {
        Date = date;
        UserId = userId;
        EntityId = entityId;
        EntityType = entityType;
        ChangeType = changeType;
        Changes = changes;
        Children = children;
    }

    public DateTimeOffset Date { get; }
    public string UserId  { get; }
    public string EntityId { get;  } 
    public string EntityType { get; }
    public string ChangeType { get; }
    public string Changes { get; }
    public string Children { get; } 
}
