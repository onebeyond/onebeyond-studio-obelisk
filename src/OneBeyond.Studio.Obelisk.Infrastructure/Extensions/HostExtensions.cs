using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OneBeyond.Studio.Crosscuts.Utilities.Templating;
using OneBeyond.Studio.Obelisk.Domain.Features.EmailTemplates.Entities;
using OneBeyond.Studio.Obelisk.Infrastructure.Data;

namespace OneBeyond.Studio.Obelisk.Infrastructure.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IHost"/> to support application-specific startup tasks.
/// </summary>
public static class HostExtensions
{
    /// <summary>
    /// Registers the email layout template for the static <see cref="HandlebarsDotNet"/> instance.
    /// Should be called after the <see cref="IHost"/> has been built and before the application starts handling requests.
    /// </summary>
    /// <param name="host">The <see cref="IHost"/> instance.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    /// <remarks>Fetches the latest template from the database if present; otherwise uses the default as fallback.</remarks>
    public static async Task RegisterLayoutTemplateAsync(this IHost host, CancellationToken cancellationToken = default)
    {
        using (var scope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        using (var dbContext = scope.ServiceProvider.GetRequiredService<DomainContext>())
        {
            var layoutTemplate = await dbContext.Set<EmailTemplate>()
                .FirstOrDefaultAsync(emailTemplate => emailTemplate.Id == PredefinedEmailTemplates.LAYOUT, cancellationToken);

            layoutTemplate ??= PredefinedEmailTemplates.DefaultEmailLayoutTemplate;

            HandlebarsLayoutTemplateManager.RegisterLayout(layoutTemplate.Id, layoutTemplate.Body);
        }
    }
}
