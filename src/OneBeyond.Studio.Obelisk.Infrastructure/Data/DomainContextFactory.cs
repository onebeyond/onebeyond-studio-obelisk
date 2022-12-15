using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace OneBeyond.Studio.Obelisk.Infrastructure.Data;

/// <summary>
/// This design-time DbContext factory is required as <see cref="DomainContext"/> contains protected paramterless ctor.
/// On the one hand this ctor is intended for building a dynamic proxy around existing fully initialized DbContext in order to support domain events.
/// On the other hand migration tooling favours it instead of the public one, but as it is not fully initialized it fails.
/// </summary>
internal sealed class DomainContextFactory : IDesignTimeDbContextFactory<DomainContext>
{
    DomainContext IDesignTimeDbContextFactory<DomainContext>.CreateDbContext(string[] args)
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? throw new Exception("ASPNETCORE_ENVIRONMENT environment variable not found.");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var dbContextOptionsBuilder = new DbContextOptionsBuilder<DomainContext>();
        dbContextOptionsBuilder.UseSqlServer(configuration.GetConnectionString("ApplicationConnectionString")!);
        var dbContextOptions = dbContextOptionsBuilder.Options;

        return new DomainContext(
            dbContextOptions,
            areDomainEventsEnabled: true); // Make sure this value matches main configuration in the Startup.cs, Whether DE support is included or not.
    }
}
