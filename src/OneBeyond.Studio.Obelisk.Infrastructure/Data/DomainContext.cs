using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OneBeyond.Studio.DataAccess.EFCore.DbContexts;
using OneBeyond.Studio.DataAccess.EFCore.DomainEvents;
using OneBeyond.Studio.Domain.SharedKernel.Entities;
using OneBeyond.Studio.Obelisk.Authentication.Application.Entities;
using SmartEnum.EFCore;

namespace OneBeyond.Studio.Obelisk.Infrastructure.Data;

public class DomainContext : IdentityDbContext<AuthUser, AuthRole, string>
{
    private readonly bool _areDomainEventsEnabled;

    public DomainContext(DbContextOptions<DomainContext> options, bool areDomainEventsEnabled)
        : base(options)
    {
        _areDomainEventsEnabled = areDomainEventsEnabled;
    }

    /// <summary>
    /// This ctor is intended for building a dynamic proxy around already existing and properly initialized <see cref="DomainContext"/>.
    /// The dynamic proxy is used for supporting domain events. Having this ctor causes a <see cref="DomainContextFactory"/> to appear
    /// for design-time configurations.
    /// </summary>
    protected DomainContext()
    {
    }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder
            .ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly())
            .SetQueryFilterOnEntities<ISoftDeletable>((entity) => !entity.IsDeleted);
        if (_areDomainEventsEnabled)
        {
            builder
                .ApplyConfiguration(new RaisedDomainEventConfiguration());
        }

        builder.ConfigureSmartEnum();
    }
}
