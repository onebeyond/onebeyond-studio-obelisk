using OneBeyond.Studio.DataAccess.EFCore.Repositories;
using OneBeyond.Studio.Domain.SharedKernel.Entities;

namespace OneBeyond.Studio.Obelisk.Infrastructure.Data;

internal sealed class AggregateRootRWRepository<TAggregateRoot, TEntity, TEntityId>
    : AggregateRootRWRepository<DomainContext, TAggregateRoot, TEntity, TEntityId>
    where TAggregateRoot : AggregateRoot<TEntity, TEntityId>
    where TEntity : DomainEntity<TEntityId>
{
    public AggregateRootRWRepository(DomainContext dbContext)
        : base(dbContext)
    {
    }
}
