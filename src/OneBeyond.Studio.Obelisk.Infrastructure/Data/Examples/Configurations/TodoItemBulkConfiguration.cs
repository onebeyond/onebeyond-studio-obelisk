using System;
using OneBeyond.Studio.Obelisk.Domain.Features.Examples.Entities;
using OneBeyond.Studio.Obelisk.Infrastructure.Data.BulkUpdate;

namespace OneBeyond.Studio.Obelisk.Infrastructure.Data.Examples.Configurations;

internal sealed class TodoItemBulkUpdateConfiguration : BulkUpdateConfiguration<TodoItem, Guid>
{
    public override EntityTypeMapping GetTypeMapping(DomainContext context)
    {
        var typeMapping = base.GetTypeMapping(context);
        return typeMapping;
    }
}
