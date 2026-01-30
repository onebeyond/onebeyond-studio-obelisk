using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using OneBeyond.Studio.Crosscuts.Options;
using OneBeyond.Studio.FileStorage.Azure.Options;
using OneBeyond.Studio.Infrastructure.Azure.MessageQueues.Options;
using OneBeyond.Studio.Obelisk.WebApi.Extensions;

namespace OneBeyond.Studio.Obelisk.WebApi.Extensions;

internal static class HealthCheckExtensions
{
    public const string SelfCheck = "self";

    public static IServiceCollection AddHealthChecks(
        this IServiceCollection services,
        IHostEnvironment environment,
        IConfiguration configuration)
    {
        var hcBuilder = services.AddHealthChecks();

        hcBuilder
            .AddSqlServer(
                configuration.GetConnectionString("ApplicationConnectionString")!,
                name: "DB-check",
                tags: ["db"]);

        hcBuilder
            .AddAzureQueueStorage(
                configuration.GetOptions<AzureMessageQueueOptions>("DomainEvents:Queue").ConnectionString!,
                name: "queue-check",
                tags: ["queue"]);

        if (!environment.IsDevelopment())
        {
            hcBuilder
                .AddAzureBlobStorage(
                    configuration.GetOptions<AzureBlobFileStorageOptions>("FileStorage:AzureBlobStorage").ConnectionString!,
                    name: "storage-check",
                    tags: ["storage"]);
        }

        return services;
    }
}
