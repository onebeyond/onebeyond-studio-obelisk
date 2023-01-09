using OneBeyond.Studio.Application.SharedKernel.DataAccessPolicies;
using OneBeyond.Studio.DataAccess.EFCore.Projections;
using OneBeyond.Studio.DataAccess.EFCore.Repositories;
using OneBeyond.Studio.Domain.SharedKernel.Entities;

namespace OneBeyond.Studio.Obelisk.Infrastructure.Data;

public class RORepository<TItem>
    : BaseRORepository<DomainContext, TItem>
    where TItem : class
{
    public RORepository(
        DomainContext dbContext,
        IRODataAccessPolicyProvider<TItem> roDataAccessPolicyProvider,
        IEntityTypeProjections<TItem> entityTypeProjections)
        : base(dbContext, roDataAccessPolicyProvider, entityTypeProjections)
    {
    }
}

public class RORepository<TEntity, TEntityId>
    : BaseRORepository<DomainContext, TEntity, TEntityId>
    where TEntity : DomainEntity<TEntityId>
    where TEntityId : notnull
{
    public RORepository(
        DomainContext dbContext,
        IRODataAccessPolicyProvider<TEntity> roDataAccessPolicyProvider,
        IEntityTypeProjections<TEntity> entityTypeProjections)
        : base(dbContext, roDataAccessPolicyProvider, entityTypeProjections)
    {
    }
}
