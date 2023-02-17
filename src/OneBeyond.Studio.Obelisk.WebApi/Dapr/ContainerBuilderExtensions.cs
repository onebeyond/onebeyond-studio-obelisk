using EnsureThat;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OneBeyond.Studio.Obelisk.WebApi.Dapr;

public static class ContainerBuilderExtensions
{
    public static IServiceCollection AddDapr(
        this IServiceCollection services,
        DaprOptions daprOptions,
        IConfiguration configuration,
        Serilog.ILogger logger)
    {
        EnsureArg.IsNotNull(services, nameof(services));
        EnsureArg.IsNotNull(configuration, nameof(configuration));
        EnsureArg.IsNotNull(daprOptions, nameof(daprOptions));

        logger.Information($"DAPR Self hosted: {daprOptions.UseSelfHosted.ToString()}");

        if (daprOptions.UseSelfHosted)
        {
            var config = daprOptions.Configuration;
            //We use Self-hosted mode for local de only
            services.AddDaprSidekick(configuration,
                options => options.Sidecar = new Man.Dapr.Sidekick.DaprSidecarOptions
                {
                    AppId = config.AppId,
                    AppSsl = config.AppSsl,
                    AppPort = config.AppPort,
                    DaprHttpPort = config.DaprHttpPort,
                    DaprGrpcPort = config.DaprGrpcPort,
                    AppProtocol = config.AppProtocol,
                    LogLevel = config.LogLevel
                });

        }

        return services;
    }
}
