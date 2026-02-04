using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning.ApiExplorer;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MoreLinq;
using OneBeyond.Studio.Application.SharedKernel.DependencyInjection;
using OneBeyond.Studio.Application.SharedKernel.DomainEvents;
using OneBeyond.Studio.Core.Mediator.DependencyInjection;
using OneBeyond.Studio.Crosscuts.Logging;
using OneBeyond.Studio.Crosscuts.Options;
using OneBeyond.Studio.DataAccess.EFCore.DependencyInjection;
using OneBeyond.Studio.EmailProviders.Folder.DependencyInjection;
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
using OneBeyond.Studio.Obelisk.Infrastructure.Data;
using OneBeyond.Studio.Obelisk.Infrastructure.Data.Seeding;
using OneBeyond.Studio.Obelisk.Infrastructure.DependencyInjection;
using OneBeyond.Studio.Obelisk.Infrastructure.Extensions;
using OneBeyond.Studio.Obelisk.WebApi.AmbientContexts;
using OneBeyond.Studio.Obelisk.WebApi.Extensions;
using OneBeyond.Studio.Obelisk.WebApi.Helpers;
using OneBeyond.Studio.Obelisk.WebApi.HostedServices;
using OneBeyond.Studio.Obelisk.WebApi.Middlewares;
using OneBeyond.Studio.Obelisk.WebApi.Middlewares.ExceptionHandling;
using OneBeyond.Studio.Obelisk.WebApi.Middlewares.Security;
using OneBeyond.Studio.Obelisk.WebApi.Swagger;
using OneBeyond.Studio.TemplateRendering.DependencyInjection;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using FolderEmailSender = OneBeyond.Studio.EmailProviders.Folder;
using SendGridEmailSender = OneBeyond.Studio.EmailProviders.SendGrid;

namespace OneBeyond.Studio.Obelisk.WebApi;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Logger used during app bootstrap
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        try
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                ?? throw new Exception("The ASPNETCORE_ENVIRONMENT variable is not set.");

            Log.Information("Obelisk starting up using environment: {RunTimeEnvironment}", environmentName);

            var builder = WebApplication.CreateBuilder(args);

            ConfigureConfiguration(builder.Configuration, environmentName);

            ConfigureLogging(builder.Host);

            builder.Host.ConfigureServices(ConfigureServices);

            // Autofac factory will automatically populate services defined above into its container
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

            builder.Host.ConfigureContainer<ContainerBuilder>(ConfigureAutofacServices);

            var app = builder.Build();

            ConfigureMiddleware(app, app.Services, builder.Configuration);

            await app.InitialiseAsync(builder.Environment, CancellationToken.None);

            await app.SeedAsync(CancellationToken.None);

            await app.RegisterLayoutTemplateAsync(CancellationToken.None);

            await app.RunAsync();
        }
        catch (Exception exception)
        when (exception is not HostAbortedException) // Used by EF migration building process
        {
            Log.Fatal(exception, "Obelisk web host terminated unexpectedly");
        }
        finally
        {
            Log.Information("Obelisk shut down complete");
            Log.CloseAndFlush();
        }
    }

    private static void ConfigureConfiguration(ConfigurationManager configuration, string environmentName)
        => configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
            .AddKeyVault("KeyVault")
            .AddEnvironmentVariables()
            .AddUserSecrets(Assembly.GetExecutingAssembly());

    private static void ConfigureLogging(IHostBuilder builder)
        => builder.UseSerilog(
            (context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

    private static void ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection services)
    {
        var configuration = hostBuilderContext.Configuration;
        var environment = hostBuilderContext.HostingEnvironment;

        services.AddOptions();

        services.AddSignalR();

        // This is required in order to restore telemetry collection. Azure agent-based collection
        // stops working as soon as AI SDK gets referenced from the project which is needed for logging to AI.
        services.AddApplicationInsightsTelemetry(
            (options) => options.ConnectionString = configuration.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING"));

        if (environment.IsDevelopment())
        {
            services.AddEmailSender(
                configuration.GetOptions<FolderEmailSender.Options.EmailSenderOptions>("EmailSender:Folder"));
        }
        else
        {
            services.AddEmailSender(
                configuration.GetOptions<SendGridEmailSender.Options.EmailSenderOptions>("EmailSender:SendGrid"));
        }

        services.AddHttpContextAccessor();

        if (!environment.IsDevelopment())
        {
            services.AddHttpsRedirection(
                (options) =>
                {
                    options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
                    options.HttpsPort = 443;
                });
        }

        services.Configure<ClientApplicationOptions>(configuration.GetSection("ClientApplication"));

        services.AddTransient<ClientApplicationLinkGenerator, ClientApplicationLinkGenerator>();

        services.AddHandlebarsTemplateRenderer();

        services.AddTransient<IApplicationClaimsService, ApplicationClaimsIdentityFactory>();

        services.AddApplicationAuthentication(environment, configuration)
            .AddUserStore<AuthUserStore>() //Used for JWT authentication
            .AddClaimsPrincipalFactory<ApplicationClaimsIdentityFactory>()
            .AddEntityFrameworkStores<DomainContext>();

        if (!environment.IsDevelopment())
        {
            services.AddAntiforgery(
            (options) =>
            {
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });
        }

        services.AddControllers()
            .AddNewtonsoftJson(
                (options) =>
                {
                    options.AddPrivateSettersSerialization();
                });

        var apiVersioningBuilder = services.AddApiVersioning(
            (options) =>
            {
                options.AssumeDefaultVersionWhenUnspecified = false;
                options.ReportApiVersions = true;
            });

        apiVersioningBuilder.AddApiExplorer(
            (options) =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        services.AddCors(
            (options) =>
            {
                options.AddDefaultPolicy(
                    (builder) =>
                    {
                        var origins = configuration
                            .GetOptions<string[]>("Cors:AllowedOrigins")
                            .Select(origin => origin.TrimEnd('/')) //trim ending slash for urls (to eliminate CORS issues)
                            .ToArray();

                        builder.WithOrigins(origins)
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    });
            });

        services.Configure<ApiBehaviorOptions>(
            (options) =>
            {
                options.SuppressInferBindingSourcesForParameters = true;
            });

        services.AddCoreMediator();

        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

        services.AddSwaggerGen(options =>
        {
            string[] methodsOrder = ["GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS", "TRACE"];
            options.OrderActionsBy(apiDesccription =>
            {
                var route = apiDesccription.ActionDescriptor.RouteValues["controller"];
                var methodIndex = Array.FindIndex(
                    methodsOrder,
                    method => method.Equals(apiDesccription.HttpMethod, StringComparison.OrdinalIgnoreCase));
                return $"{route}_{methodIndex}";
            });
        });

        services.AddDataAccessSeeding(
            configuration.GetOptions<IdentitiesSeederOptions>("Identities:Seeding"));

        services.AddDataAccess(
            configuration,
            (dataAccessBuilder) =>
                dataAccessBuilder
                    // If DE support is enabled, make sure the DomainContextFactory does also when constructing the DomainContext.
                    .WithDomainEvents(isReceiverHost: true));
                    //.WithUnitOfWork());

        services.AddHostedService<DomainEventRelayJob>();

        services.AddEntityTypeProjections(typeof(Infrastructure.AssemblyMark).Assembly);

        services.AddFileStorage(
            (builder) =>
                _ = environment.IsDevelopment()
                    ? builder.UseFileSystem(configuration.GetOptions<FileSystemFileStorageOptions>("FileStorage:FileSystem"))
                    : builder.UseAzureBlobs(configuration.GetOptions<AzureBlobFileStorageOptions>("FileStorage:AzureBlobStorage")));


        services.AddMixedSourceBinder(); //Mixed source binder used to gather commands, queries and dtos in controllers from mixed sources: body and route

        services.AddLocalization(options => options.ResourcesPath = "Localizations/Resources");

        services.AddHealthChecks(environment, configuration);
    }

    private static void ConfigureAutofacServices(HostBuilderContext hostBuilderContext, ContainerBuilder containerBuilder)
    {
        var configuration = hostBuilderContext.Configuration;

        containerBuilder.AddAmbientContextAccessor<AmbientContextAccessor, AmbientContext>();

        containerBuilder.AddApplication();

        containerBuilder.AddAuthApplication();

        containerBuilder.AddAzureMessageQueue<RaisedDomainEvent>(
            configuration.GetOptions<AzureMessageQueueOptions>("DomainEvents:Queue"));
    }

    private static void ConfigureMiddleware(
        IApplicationBuilder app,
        IServiceProvider services,
        IConfiguration configuration)
    {
        var loggerFactory = services.GetRequiredService<ILoggerFactory>();
        LogManager.Configure(loggerFactory);

        app.UseSerilogRequestLogging();

        var environment = services.GetRequiredService<IWebHostEnvironment>();

        app.UseCookiePolicy(
            new CookiePolicyOptions
            {
                Secure = environment.IsDevelopment()
                    ? CookieSecurePolicy.SameAsRequest
                    : CookieSecurePolicy.Always
            });

        if (environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseSecurityHeadersMiddleware(
            new SecurityHeadersBuilder()
                .AddSecurityPolicyFromConfiguration(configuration.GetSection("SecurityHeaders")));

        app.UseUnhandledExceptionLogging();

        app.UseMiddleware<ErrorResultGeneratorMiddleware>();

        app.UseExceptionHandling();

        app.UseStaticFiles();

        app.UseRouting();

        //app.UseUnitOfWork();

        app.UseCors();

        var cultures = configuration.GetOptions<string[]>("Localization:SupportedCultures");
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

        app.UseSwaggerUI(
            (options) =>
            {
                var provider = services.GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint(
                        $"/swagger/{description.GroupName}/swagger.json",
                        $"{SwaggerConstants.APITitle} {description.GroupName.ToUpperInvariant()}");

                    // Set Swagger UI as the homepage
                    options.RoutePrefix = string.Empty;
                }
            });

        app.UseEndpoints(
            (endpoints) =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
                {
                    Predicate = r => r.Name.Contains(HealthCheckExtensions.SelfCheck)
                });
            });
    }
}
