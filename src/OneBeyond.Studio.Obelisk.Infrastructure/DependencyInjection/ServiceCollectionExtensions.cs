using System;
using EnsureThat;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OneBeyond.Studio.Crosscuts.Options;
using OneBeyond.Studio.DataAccess.EFCore.DependencyInjection;
using OneBeyond.Studio.DataAccess.EFCore.Options;
using OneBeyond.Studio.Obelisk.Application.Services.Seeding;
using OneBeyond.Studio.Obelisk.Infrastructure.Data;
using OneBeyond.Studio.Obelisk.Infrastructure.Data.Seeding;

namespace OneBeyond.Studio.Obelisk.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Data Access for DI
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="cloudDeployment">This parameter is set to true by default. This allows for retries optimised for sql server, such that under load, the application continues to perform.</param>
    /// <param name="configureDataAccessBuilder"></param>
    /// <returns></returns>
    public static IServiceCollection AddDataAccess(
        this IServiceCollection services,
        IConfiguration configuration,        
        Action<IDataAccessBuilder>? configureDataAccessBuilder = default,
        bool cloudDeployment = true)
    {
        EnsureArg.IsNotNull(services, nameof(services));
        EnsureArg.IsNotNull(configuration, nameof(configuration));

        var coreDataAccessBuilder = services.AddDataAccess<DomainContext>(
            configuration.GetOptions<DataAccessOptions>("Infrastructure"),
            (serviceProvider, dbContextOptionsBuilder)
                => dbContextOptionsBuilder
                    .UseSqlServer(configuration.GetConnectionString("ApplicationConnectionString")!, 
                    options => {
                        if (cloudDeployment)
                        {
                            options.EnableRetryOnFailure();
                        }
                    }),
            (serviceProvider, dbContextOptions, areDomainEventsEnabled)
                => new DomainContext(dbContextOptions, areDomainEventsEnabled));

        var dataAccessBuilder = new DataAccessBuilder(
            services,
            coreDataAccessBuilder);

        configureDataAccessBuilder?.Invoke(dataAccessBuilder);

        return services;
    }

    public static IServiceCollection AddDataAccessSeeding(
        this IServiceCollection services,
        IdentitiesSeederOptions identitySeedingOptions)
    {
        EnsureArg.IsNotNull(services, nameof(services));
        EnsureArg.IsNotNull(identitySeedingOptions, nameof(identitySeedingOptions));

        services.AddSingleton(identitySeedingOptions);

        services.AddScoped<INotificationHandler<SeedApplication>, IdentitiesSeeder>();

        services.AddScoped<INotificationHandler<SeedApplication>, EmailTemplatesSeeder>();

        return services;
    }
}
