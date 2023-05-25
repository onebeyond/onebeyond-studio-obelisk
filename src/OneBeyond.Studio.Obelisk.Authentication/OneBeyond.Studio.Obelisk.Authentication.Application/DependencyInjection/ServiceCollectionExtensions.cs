using System;
using EnsureThat;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using OneBeyond.Studio.Crosscuts.Options;
using OneBeyond.Studio.Obelisk.Authentication.Application.Entities;
using OneBeyond.Studio.Obelisk.Authentication.Application.JwtAuthentication.DependencyInjection;
using OneBeyond.Studio.Obelisk.Authentication.Application.Options;
using OneBeyond.Studio.Obelisk.Authentication.Application.Schemes;
using OneBeyond.Studio.Obelisk.Authentication.Application.Services.AuthenticationFlows;
using OneBeyond.Studio.Obelisk.Authentication.Domain.JwtAuthentication;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IdentityBuilder AddApplicationAuthentication(
        this IServiceCollection services,
        IHostEnvironment hostEnvironment,
        IConfiguration configuration)
    {
        EnsureArg.IsNotNull(services, nameof(services));
        EnsureArg.IsNotNull(hostEnvironment, nameof(hostEnvironment));
        EnsureArg.IsNotNull(configuration, nameof(configuration));

        services.Configure<IdentityOptions>(
            (options) =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 10;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequiredUniqueChars = 2;
                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = SessionConstants.SessionDuration;
                if (int.TryParse(configuration.GetOptions<string>("LockoutMaxFailedAccessAttempts"), out int maxAttempts))
                {
                    options.Lockout.MaxFailedAccessAttempts = maxAttempts;
                }
                else
                {
                    options.Lockout.MaxFailedAccessAttempts = 5;
                }
                options.Lockout.AllowedForNewUsers = true;
            });

        services.AddScoped<CookieAuthenticationFlow>();

        var identityBuilder = services.AddIdentity<AuthUser, AuthRole>()
            .AddDefaultTokenProviders()
            .AddRoleManager<RoleManager<AuthRole>>()
            .AddUserManager<UserManager<AuthUser>>();

        //NOTE: AddIdentity will overwrite some of the settings e.g. LoginPath
        //So this call must be made after services.AddIdentity
        services.ConfigureApplicationCookie(
            (options) =>
            {
                options.Cookie.Name = SessionConstants.CookieName;
                options.Cookie.HttpOnly = true;
                if (configuration.GetOptions<CookieAuthNOptions>("CookieAuthN").AllowCrossSiteCookies)
                {
                    options.Cookie.SameSite = SameSiteMode.None;
                }
                options.ExpireTimeSpan = SessionConstants.SessionDuration;
                options.SlidingExpiration = true;
                options.EventsType = typeof(CookieAuthenticationFlow);
            });
        services.AddSingleton((_) =>
            configuration.GetOptions<CookieAuthNOptions>("CookieAuthN"));

        //NOTE: Order is important
        //App auth uses Identity which will override any configured options for forwarding
        //If using JWT and Identity combined, UseCombinedScheme must come last
        services.AddJwtAuthentication(
            configuration.GetOptions<JwtAuthenticationOptions>("Jwt"));

        services.UseCombinedScheme<IdentityApplicationScheme, JWTScheme>();

        services.AddAuthentication()
            .AddMicrosoftIdentityWebApp(configuration, "AzureAd", cookieScheme: null);

        return identityBuilder;
    }

    private static void UseCombinedScheme<S1, S2>(
        this IServiceCollection services,
        Action<AuthenticationOptions>? customSchemeConfiguration = null)
        where S1 : IAuthenticationScheme, new()
        where S2 : IAuthenticationScheme, new()
    {
        EnsureArg.IsNotNull(services, nameof(services));

        var s1 = new S1();
        var combinedSchemeName = s1.SchemeName + new S2().SchemeName;

        //If no specific authentication configuration is passed
        //the scheme should be configured to use the overridden authentication logic of the combined handler
        //otherwise, s1 handles all other requests
        var schemeConfiguration = customSchemeConfiguration is null
            ? (options) =>
                {
                    options.DefaultScheme = s1.SchemeName;
                    options.DefaultAuthenticateScheme = combinedSchemeName;
                }
        : customSchemeConfiguration;

        services.AddAuthentication(schemeConfiguration)
            .AddScheme<AuthenticationSchemeOptions, CombinedAuthenticationScheme<S1, S2>>(
                combinedSchemeName,
                configureOptions: null
            );
    }
}
