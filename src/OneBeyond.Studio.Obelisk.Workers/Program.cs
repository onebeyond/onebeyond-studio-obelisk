using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OneBeyond.Studio.Application.SharedKernel.AmbientContexts;
using OneBeyond.Studio.Application.SharedKernel.DependencyInjection;
using OneBeyond.Studio.Core.Mediator.DependencyInjection;
using OneBeyond.Studio.Crosscuts.Logging;
using OneBeyond.Studio.Crosscuts.Options;
using OneBeyond.Studio.DataAccess.EFCore.DependencyInjection;
using OneBeyond.Studio.EmailProviders.Folder.DependencyInjection;
using OneBeyond.Studio.EmailProviders.SendGrid.DependencyInjection;
using OneBeyond.Studio.Infrastructure.Azure.KeyVault.Configurations;
using OneBeyond.Studio.Obelisk.Application.DependencyInjection;
using OneBeyond.Studio.Obelisk.Authentication.Application.JwtAuthentication.DependencyInjection;
using OneBeyond.Studio.Obelisk.Infrastructure.DependencyInjection;
using OneBeyond.Studio.Obelisk.Infrastructure.Extensions;
using OneBeyond.Studio.Obelisk.Workers.AmbientContexts;
using OneBeyond.Studio.TemplateRendering.DependencyInjection;
using Serilog;
using FolderEmailSender = OneBeyond.Studio.EmailProviders.Folder;
using SendGridEmailSender = OneBeyond.Studio.EmailProviders.SendGrid;

namespace OneBeyond.Studio.Obelisk.Workers;

internal static class Program
{
    public static async Task Main(string[] _)
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
                .Build())
            {
                Log.Information("Azure Function host is starting");

                LogManager.Configure(host.Services.GetRequiredService<ILoggerFactory>());

                await host.RegisterLayoutTemplateAsync(CancellationToken.None);
                await host.RunAsync();

                Log.Information("Azure Function host stopped");
            }
        }
        catch (Exception exception)
        {
            Log.Fatal(exception, "Azure Function host terminated unexpectedly");
            await Log.CloseAndFlushAsync();
        }
    }

    private static void ConfigureServiceCollection(
        HostBuilderContext hostBuilderContext,
        IServiceCollection serviceCollection)
    {
        var configuration = hostBuilderContext.Configuration;
        var environment = hostBuilderContext.HostingEnvironment;

        serviceCollection.AddCoreMediator();

        serviceCollection.AddDataAccess(
                configuration,
                (dataAccessBuilder) => dataAccessBuilder.WithDomainEvents())
            .AddEntityTypeProjections(typeof(Infrastructure.AssemblyMark).Assembly);

        serviceCollection.AddJwtBackgroundServices();

        serviceCollection.AddHandlebarsTemplateRenderer();

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
        ConfigureSerilog(hostBuilderContext, serviceCollection);
    }

    private static void ConfigureSerilog(
        HostBuilderContext hostBuilderContext,
        IServiceCollection serviceCollection)
    {
        var loggerConfiguration = new LoggerConfiguration()
               .ReadFrom.Configuration(hostBuilderContext.Configuration);

        Log.Logger = hostBuilderContext.HostingEnvironment.IsDevelopment()
            ? loggerConfiguration.CreateLogger()
            : loggerConfiguration
                .Enrich.FromLogContext()
                // NOTE: the line below needs to be hardcoded even if the same property is already set in the configuration JSON
                .Enrich.WithProperty("ApplicationExecutable", "Workers")
                .WriteTo.ApplicationInsights(
                    TelemetryConfiguration.CreateDefault(),
                    TelemetryConverter.Traces)
                .CreateLogger();

        serviceCollection.AddLogging(
            cfg => cfg.AddSerilog(Log.Logger, true));
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
}
