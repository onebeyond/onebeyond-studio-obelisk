using System;
using System.Reflection;
using Autofac.Extensions.DependencyInjection;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OneBeyond.Studio.Application.SharedKernel.AmbientContexts;
using OneBeyond.Studio.Application.SharedKernel.DependencyInjection;
using OneBeyond.Studio.Core.Mediator.DependencyInjection;
using OneBeyond.Studio.Crosscuts.Logging;
using OneBeyond.Studio.Crosscuts.Options;
using OneBeyond.Studio.Crosscuts.Utilities.Templating;
using OneBeyond.Studio.DataAccess.EFCore.DependencyInjection;
using OneBeyond.Studio.EmailProviders.Folder.DependencyInjection;
using OneBeyond.Studio.EmailProviders.SendGrid.DependencyInjection;
using OneBeyond.Studio.Infrastructure.Azure.KeyVault.Configurations;
using OneBeyond.Studio.Obelisk.Application.DependencyInjection;
using OneBeyond.Studio.Obelisk.Authentication.Application.JwtAuthentication.DependencyInjection;
using OneBeyond.Studio.Obelisk.Infrastructure.DependencyInjection;
using OneBeyond.Studio.Obelisk.Workers.AmbientContexts;
using Serilog;
using FolderEmailSender = OneBeyond.Studio.EmailProviders.Folder;
using SendGridEmailSender = OneBeyond.Studio.EmailProviders.SendGrid;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureContainer(new AutofacServiceProviderFactory(), containerBuilder =>
{
    containerBuilder.AddApplication();
    containerBuilder.AddAmbientContextAccessor<AmbientContextAccessor, AmbientContext>();
});


builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json")
    .AddJsonFile(
        $"appsettings.{builder.Environment.EnvironmentName}.json",
        optional: true)
    .AddEnvironmentVariables()
    .AddKeyVault("KeyVault");

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly());
}

builder.Services.AddCoreMediator();

builder.Services.AddDataAccess(
        builder.Configuration,
        (dataAccessBuilder) => dataAccessBuilder.WithDomainEvents())
    .AddEntityTypeProjections(typeof(OneBeyond.Studio.Obelisk.Infrastructure.AssemblyMark).Assembly);

builder.Services.AddJwtBackgroundServices();

builder.Services.AddTransient<ITemplateRenderer, HandleBarsTemplateRenderer>();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEmailSender(
        builder.Configuration
            .GetOptions<FolderEmailSender.Options.EmailSenderOptions>("EmailSender:Folder"));
}
else
{
    builder.Services.AddEmailSender(
        builder.Configuration.GetOptions<SendGridEmailSender.Options.EmailSenderOptions>(
            "EmailSender:SendGrid"));
}

builder.Services.AddApplicationInsightsTelemetryWorkerService();

var loggerConfiguration = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration);

Log.Logger = builder.Environment.IsDevelopment()
    ? loggerConfiguration.CreateLogger()
    : loggerConfiguration
        .Enrich.FromLogContext()
        // NOTE: the line below needs to be hardcoded even if the same property is already set in the configuration JSON
        .Enrich.WithProperty("ApplicationExecutable", "Workers")
        .WriteTo.ApplicationInsights(
            TelemetryConfiguration.CreateDefault(),
            TelemetryConverter.Traces)
        .CreateLogger();

builder.Services.AddLogging(cfg => cfg.AddSerilog(Log.Logger, true));

var host = builder.Build();

LogManager.Configure(host.Services.GetRequiredService<ILoggerFactory>());
var logger = LogManager.CreateLogger<Program>();

try
{
    logger.LogInformation("Azure Function host is starting");
    await host.RunAsync();
    logger.LogInformation("Azure Function host stopped");
}
catch (Exception exception)
{
    logger.LogCritical(exception, "Azure Function host terminated unexpectedly");
}
