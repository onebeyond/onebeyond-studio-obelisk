using System;
using System.Reflection;
using Autofac.Extensions.DependencyInjection;
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
using FolderEmailSender = OneBeyond.Studio.EmailProviders.Folder;
using SendGridEmailSender = OneBeyond.Studio.EmailProviders.SendGrid;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureContainer(new AutofacServiceProviderFactory(), containerBuilder =>
{
    containerBuilder.AddApplication();
    containerBuilder.AddAmbientContextAccessor<AmbientContextAccessor, AmbientContext>();
});

builder.Configuration.AddKeyVault("KeyVault");

builder.AddServiceDefaults();

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
