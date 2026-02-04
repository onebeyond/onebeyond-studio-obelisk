using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Asp.Versioning.ApiExplorer;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OneBeyond.Studio.Application.SharedKernel.DependencyInjection;
using OneBeyond.Studio.Application.SharedKernel.DomainEvents;
using OneBeyond.Studio.Core.Mediator.DependencyInjection;
using OneBeyond.Studio.Crosscuts.Logging;
using OneBeyond.Studio.Crosscuts.Options;
using OneBeyond.Studio.Crosscuts.Utilities.Templating;
using OneBeyond.Studio.DataAccess.EFCore.DependencyInjection;
using OneBeyond.Studio.EmailProviders.Folder.DependencyInjection;
using OneBeyond.Studio.EmailProviders.Folder.Options;
using OneBeyond.Studio.EmailProviders.SendGrid.DependencyInjection;
using OneBeyond.Studio.FileStorage.Azure.DependencyInjection;
using OneBeyond.Studio.FileStorage.Azure.Options;
using OneBeyond.Studio.FileStorage.FileSystem.DependencyInjection;
using OneBeyond.Studio.FileStorage.FileSystem.Options;
using OneBeyond.Studio.FileStorage.Infrastructure.DependencyInjection;
using OneBeyond.Studio.Hosting.AspNet.ModelBinders.MixedSource.DependencyInjection;
using OneBeyond.Studio.Infrastructure.Azure.KeyVault.Configurations;
using OneBeyond.Studio.Infrastructure.Azure.MessageQueues.DependencyInjection;
using OneBeyond.Studio.Infrastructure.Azure.MessageQueues.Options;
using OneBeyond.Studio.Obelisk.Application.DependencyInjection;
using OneBeyond.Studio.Obelisk.Application.Services.AmbientContexts;
using OneBeyond.Studio.Obelisk.Authentication.Application.DependencyInjection;
using OneBeyond.Studio.Obelisk.Authentication.Application.Services.ApplicationClaims;
using OneBeyond.Studio.Obelisk.Infrastructure;
using OneBeyond.Studio.Obelisk.Infrastructure.Data;
using OneBeyond.Studio.Obelisk.Infrastructure.Data.Seeding;
using OneBeyond.Studio.Obelisk.Infrastructure.DependencyInjection;
using OneBeyond.Studio.Obelisk.WebApi.AmbientContexts;
using OneBeyond.Studio.Obelisk.WebApi.Extensions;
using OneBeyond.Studio.Obelisk.WebApi.Helpers;
using OneBeyond.Studio.Obelisk.WebApi.HostedServices;
using OneBeyond.Studio.Obelisk.WebApi.Middlewares;
using OneBeyond.Studio.Obelisk.WebApi.Middlewares.ExceptionHandling;
using OneBeyond.Studio.Obelisk.WebApi.Middlewares.Security;
using OneBeyond.Studio.Obelisk.WebApi.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;


var builder = WebApplication.CreateBuilder(args);

// Add Key Vault configuration source
builder.Configuration.AddKeyVault("KeyVault");

// Autofac factory will automatically populate services defined above into its container
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Host.ConfigureContainer<ContainerBuilder>((hostBuilderContext, containerBuilder) =>
{
    var configuration = hostBuilderContext.Configuration;

    containerBuilder.AddAmbientContextAccessor<AmbientContextAccessor, AmbientContext>();

    containerBuilder.AddApplication();

    containerBuilder.AddAuthApplication();

    containerBuilder.AddAzureMessageQueue<RaisedDomainEvent>(
        configuration.GetOptions<AzureMessageQueueOptions>("DomainEvents:Queue"));
});

builder.AddServiceDefaults();

builder.Services.AddOptions();

builder.Services.AddSignalR();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEmailSender(
        builder.Configuration.GetOptions<EmailSenderOptions>("EmailSender:Folder"));
}
else
{
    builder.Services.AddEmailSender(
        builder.Configuration.GetOptions<OneBeyond.Studio.EmailProviders.SendGrid.Options.EmailSenderOptions>(
            "EmailSender:SendGrid"));
}

builder.Services.AddHttpContextAccessor();

if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddHttpsRedirection(options =>
    {
        options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
        options.HttpsPort = 443;
    });
}

builder.Services.Configure<ClientApplicationOptions>(builder.Configuration.GetSection("ClientApplication"));

builder.Services.AddTransient<ClientApplicationLinkGenerator, ClientApplicationLinkGenerator>();

builder.Services.AddTransient<ITemplateRenderer, HandleBarsTemplateRenderer>();

builder.Services.AddTransient<IApplicationClaimsService, ApplicationClaimsIdentityFactory>();

builder.Services.AddApplicationAuthentication(builder.Environment, builder.Configuration)
    .AddUserStore<AuthUserStore>() //Used for JWT authentication
    .AddClaimsPrincipalFactory<ApplicationClaimsIdentityFactory>()
    .AddEntityFrameworkStores<DomainContext>();

if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddAntiforgery(options =>
    {
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });
}

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.AddPrivateSettersSerialization();
    });

var apiVersioningBuilder = builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = false;
    options.ReportApiVersions = true;
});

apiVersioningBuilder.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        var origins = builder.Configuration
            .GetOptions<string[]>("Cors:AllowedOrigins")
            .Select(origin => origin.TrimEnd('/')) //trim ending slash for urls (to eliminate CORS issues)
            .ToArray();

        policyBuilder.WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressInferBindingSourcesForParameters = true;
});

builder.Services.AddCoreMediator();

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

builder.Services.AddSwaggerGen(options =>
{
    string[] methodsOrder = ["GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS", "TRACE"];

    options.OrderActionsBy(apiDescription =>
    {
        var route = apiDescription.ActionDescriptor.RouteValues["controller"];

        var methodIndex = Array.FindIndex(
            methodsOrder,
            method => method.Equals(apiDescription.HttpMethod, StringComparison.OrdinalIgnoreCase));

        return $"{route}_{methodIndex}";
    });
});

builder.Services.AddDataAccessSeeding(
    builder.Configuration.GetOptions<IdentitiesSeederOptions>("Identities:Seeding"));

builder.Services.AddDataAccess(
    builder.Configuration,
    dataAccessBuilder =>
        dataAccessBuilder
            // If DE support is enabled, make sure the DomainContextFactory does also when constructing the DomainContext.
            .WithDomainEvents(isReceiverHost: true));
//.WithUnitOfWork());

builder.Services.AddHostedService<DomainEventRelayJob>();

builder.Services.AddEntityTypeProjections(typeof(AssemblyMark).Assembly);

builder.Services.AddFileStorage(storageBuilder =>
    _ = builder.Environment.IsDevelopment()
        ? storageBuilder.UseFileSystem(
            builder.Configuration.GetOptions<FileSystemFileStorageOptions>("FileStorage:FileSystem"))
        : storageBuilder.UseAzureBlobs(
            builder.Configuration.GetOptions<AzureBlobFileStorageOptions>("FileStorage:AzureBlobStorage")));


builder.Services
    .AddMixedSourceBinder(); //Mixed source binder used to gather commands, queries and dtos in controllers from mixed sources: body and route

builder.Services.AddLocalization(options => options.ResourcesPath = "Localizations/Resources");

builder.AddAdditionalHealthChecks();

var app = builder.Build();

var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
LogManager.Configure(loggerFactory);

app.UseCookiePolicy(
    new CookiePolicyOptions
    {
        Secure = app.Environment.IsDevelopment()
            ? CookieSecurePolicy.SameAsRequest
            : CookieSecurePolicy.Always
    });

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSecurityHeadersMiddleware(
    new SecurityHeadersBuilder()
        .AddSecurityPolicyFromConfiguration(builder.Configuration.GetSection("SecurityHeaders")));

app.UseUnhandledExceptionLogging();

app.UseMiddleware<ErrorResultGeneratorMiddleware>();

app.UseExceptionHandling();

app.UseStaticFiles();

app.UseRouting();

//app.UseUnitOfWork();

app.UseCors();

var cultures = builder.Configuration.GetOptions<string[]>("Localization:SupportedCultures");

if (cultures.Any())
{
    app.UseRequestLocalization(
        new RequestLocalizationOptions()
            .SetDefaultCulture(cultures.First())
            .AddSupportedCultures(cultures)
            .AddSupportedUICultures(cultures));
}

app.UseAuthentication();

app.UseAuthorization();

app.UseSwagger();

app.UseSwaggerUI(options =>
{
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

    foreach (var description in provider.ApiVersionDescriptions)
    {
        options.SwaggerEndpoint(
            $"/swagger/{description.GroupName}/swagger.json",
            $"{SwaggerConstants.APITitle} {description.GroupName.ToUpperInvariant()}");

        // Set Swagger UI as the homepage
        options.RoutePrefix = string.Empty;
    }
});

app.MapControllers();
app.MapDefaultEndpoints();

try
{
    await app.InitialiseAsync(builder.Environment, CancellationToken.None);
    await app.SeedAsync(CancellationToken.None);
    await app.RunAsync();
}
catch (Exception exception) when (exception is not HostAbortedException) // Used by EF migration building process
{
    app.Logger.LogCritical(exception, "Obelisk web host terminated unexpectedly");
}
finally
{
    app.Logger.LogInformation("Obelisk shut down complete");
}
