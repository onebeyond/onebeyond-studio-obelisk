using System;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OneBeyond.Studio.Application.SharedKernel.AmbientContexts;
using OneBeyond.Studio.Application.SharedKernel.DependencyInjection;
using OneBeyond.Studio.Crosscuts.Logging;
using OneBeyond.Studio.Crosscuts.Options;
using OneBeyond.Studio.Crosscuts.Utilities.Templating;
using OneBeyond.Studio.DataAccess.EFCore.DependencyInjection;
using OneBeyond.Studio.EmailProviders.Folder.DependencyInjection;
using OneBeyond.Studio.EmailProviders.SendGrid.DependencyInjection;
using OneBeyond.Studio.Infrastructure.Azure.KeyVault.Configurations;
using OneBeyond.Studio.Obelisk.Application.DependencyInjection;
using OneBeyond.Studio.Obelisk.Infrastructure.DependencyInjection;
using OneBeyond.Studio.Obelisk.Workers.AmbientContexts;
using Serilog;

using SendGridEmailSender = OneBeyond.Studio.EmailProviders.SendGrid;
using FolderEmailSender = OneBeyond.Studio.EmailProviders.Folder;

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
        var environment = hostBuilderContext.HostingEnvironment;

        serviceCollection.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        serviceCollection.AddDataAccess(
                configuration,
                (dataAccessBuilder) => dataAccessBuilder.WithDomainEvents())
            .AddEntityTypeProjections(typeof(Infrastructure.AssemblyMark).Assembly);

        serviceCollection.AddAutoMapper(typeof(Application.AssemblyMark).Assembly);

        serviceCollection.AddTransient<ITemplateRenderer, HandleBarsTemplateRenderer>();

        if (environment.IsDevelopment())
        {
            serviceCollection.AddEmailSender(
                configuration.GetOptions<FolderEmailSender.Options.EmailSenderOptions>("EmailSender:Folder"));
        }
        else
        {
            serviceCollection.AddEmailSender(
                configuration.GetOptions<SendGridEmailSender.Options.EmailSenderOptions>("EmailSender:SendGrid"));
        }

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
