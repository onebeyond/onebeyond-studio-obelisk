using System.Text;
using EnsureThat;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OneBeyond.Studio.Obelisk.Authentication.Application.Services.JwtAuthentication;
using OneBeyond.Studio.Obelisk.Authentication.Domain.JwtAuthentication;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.JwtAuthentication.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        JwtAuthenticationOptions jwtAuthenticationOptions)
    {
        EnsureArg.IsNotNull(services, nameof(services));
        EnsureArg.IsNotNull(jwtAuthenticationOptions, nameof(jwtAuthenticationOptions));

        jwtAuthenticationOptions.EnsureIsValid();

        services.AddAuthentication()
            .AddJwtBearer(cfg =>
            {
                cfg.SaveToken = true;
                cfg.ForwardChallenge = JwtBearerDefaults.AuthenticationScheme;
                cfg.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = jwtAuthenticationOptions.Issuer,
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAuthenticationOptions.Secret!))
                };
            });

        services.AddSingleton(jwtAuthenticationOptions);

        services.AddTransient<IJwtTokenService, JwtTokenService>();

        return services;
    }

    public static IServiceCollection AddJwtBackgroundServices(this IServiceCollection services)
    {
        services.AddTransient<IJwtCleardownService, JwtCleardownServce>();
        return services;
    }
}
