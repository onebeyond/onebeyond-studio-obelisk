using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OneBeyond.Studio.Crosscuts.Options;
using OneBeyond.Studio.FileStorage.Azure.Options;
using OneBeyond.Studio.Infrastructure.Azure.MessageQueues.Options;

namespace OneBeyond.Studio.Obelisk.WebApi.Extensions;

internal static class HealthCheckExtensions
{
    /// <summary>
    /// Configures additional app-specific health checks. A basic liveness health check is already configured by the
    /// ServiceDefaults.
    /// </summary>
    public static TBuilder AddAdditionalHealthChecks<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        var hcBuilder = builder.Services.AddHealthChecks();

        hcBuilder
            .AddSqlServer(
                builder.Configuration.GetConnectionString("ApplicationConnectionString")!,
                name: "DB-check",
                tags: ["db"]);

        hcBuilder
            .AddAzureQueueStorage(
                builder.Configuration.GetOptions<AzureMessageQueueOptions>("DomainEvents:Queue").ConnectionString!,
                name: "queue-check",
                tags: ["queue"]);

        if (!builder.Environment.IsDevelopment())
        {
            hcBuilder
                .AddAzureBlobStorage(
                    builder.Configuration.GetOptions<AzureBlobFileStorageOptions>("FileStorage:AzureBlobStorage")
                        .ConnectionString!,
                    name: "storage-check",
                    tags: ["storage"]);
        }

        return builder;
    }
}
