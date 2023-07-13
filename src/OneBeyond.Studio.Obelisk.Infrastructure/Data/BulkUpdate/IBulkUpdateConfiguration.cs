using OneBeyond.Studio.Domain.SharedKernel.Entities;

namespace OneBeyond.Studio.Obelisk.Infrastructure.Data.BulkUpdate;

public interface IBulkUpdateConfiguration<TAggregateRoot, TAggregateRootId>
    where TAggregateRoot : AggregateRoot<TAggregateRootId>
    where TAggregateRootId : notnull
{
    public EntityTypeMapping GetTypeMapping(DomainContext context);
}
