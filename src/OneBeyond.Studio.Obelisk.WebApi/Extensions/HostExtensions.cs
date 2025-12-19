using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OneBeyond.Studio.Core.Mediator;
using OneBeyond.Studio.Obelisk.Application.Services.Seeding;
using OneBeyond.Studio.Obelisk.Infrastructure.Data;

namespace OneBeyond.Studio.Obelisk.WebApi.Extensions;

public static class HostExtensions
{
    public static async Task<IHost> InitialiseAsync(
        this IHost host,
        IHostEnvironment hostEnvironment,
        CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(host, nameof(host));
        EnsureArg.IsNotNull(hostEnvironment, nameof(hostEnvironment));

        if (hostEnvironment.IsDevelopment())
        {
            using (var scope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            using (var dbContext = scope.ServiceProvider.GetRequiredService<DomainContext>())
            {
                await dbContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        return host;
    }

    public static async Task<IHost> SeedAsync(this IHost host, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(host, nameof(host));

        using (var scope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await mediator.NotifyAsync(new SeedApplication(), cancellationToken).ConfigureAwait(false);
            return host;
        }
    }
}
