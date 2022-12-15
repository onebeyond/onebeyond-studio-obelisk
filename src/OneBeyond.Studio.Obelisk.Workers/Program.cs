using System;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OneBeyond.Studio.Crosscuts.Logging;
using OneBeyond.Studio.DataAccess.EFCore.DependencyInjection;
using OneBeyond.Studio.Domain.SharedKernel.AmbientContexts;
using OneBeyond.Studio.Domain.SharedKernel.DependencyInjection;
using OneBeyond.Studio.Infrastructure.Azure.KeyVault.Configurations;
using OneBeyond.Studio.Obelisk.Application.DependencyInjection;
using OneBeyond.Studio.Obelisk.Infrastructure.DependencyInjection;
using OneBeyond.Studio.Obelisk.Workers.AmbientContexts;
using Serilog;

namespace OneBeyond.Studio.Obelisk.Workers;

internal static class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        try
        {
            using (var host = new HostBuilder()
                .UseServiceProviderFactory<ContainerBuilder>(new AutofacServiceProviderFactory())
                .ConfigureAppConfiguration(ConfigureAppConfiguration)
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(ConfigureServiceCollection)
                .ConfigureContainer<ContainerBuilder>(ConfigureContainerBuilder)
                .UseSerilog(ConfigureSerilog)
                .Build())
            {
                Log.Information("Azure Function host is starting");

                LogManager.Configure(host.Services.GetRequiredService<ILoggerFactory>());

                host.Run();

                Log.Information("Azure Function host stopped");
            }
        }
        catch (Exception exception)
        {
            Log.Fatal(exception, "Azure Function host terminated unexpectedly");
            Log.CloseAndFlush();
        }
    }

    private static void ConfigureServiceCollection(
        HostBuilderContext hostBuilderContext,
        IServiceCollection serviceCollection)
    {
        var configuration = hostBuilderContext.Configuration;

        serviceCollection.AddMediatR(Assembly.GetExecutingAssembly());

        serviceCollection.AddDataAccess(
                configuration,
                (dataAccessBuilder) => dataAccessBuilder.WithDomainEvents())
            .AddEntityTypeProjections(typeof(Infrastructure.AssemblyMark).Assembly);

        serviceCollection.AddAutoMapper(typeof(Application.AssemblyMark).Assembly);

        serviceCollection.AddApplicationInsightsTelemetryWorkerService();
    }

    private static void ConfigureContainerBuilder(
        HostBuilderContext _,
        ContainerBuilder containerBuilder)
    {
        containerBuilder.AddApplication();

        containerBuilder.AddAmbientContextAccessor<AmbientContextAccessor, AmbientContext>();
    }

    private static void ConfigureAppConfiguration(
        HostBuilderContext hostBuilderContext,
        IConfigurationBuilder builder)
    {
        builder
            .SetBasePath(hostBuilderContext.HostingEnvironment.ContentRootPath)
            .AddJsonFile("appsettings.json")
            .AddJsonFile(
                $"appsettings.{hostBuilderContext.HostingEnvironment.EnvironmentName}.json",
                optional: true)
            .AddEnvironmentVariables()
            .AddKeyVault("KeyVault");

        if (hostBuilderContext.HostingEnvironment.IsDevelopment())
        {
            builder.AddUserSecrets(Assembly.GetExecutingAssembly());
        }
    }

    private static void ConfigureSerilog(
        HostBuilderContext hostBuilderContext,
        LoggerConfiguration loggerConfiguration)
        => loggerConfiguration.ReadFrom.Configuration(hostBuilderContext.Configuration);
}
