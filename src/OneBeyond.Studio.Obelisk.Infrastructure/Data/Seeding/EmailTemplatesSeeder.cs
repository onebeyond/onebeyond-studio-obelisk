using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.EntityFrameworkCore;
using OneBeyond.Studio.Core.Mediator.Notifications;
using OneBeyond.Studio.Obelisk.Application.Services.Seeding;
using OneBeyond.Studio.Obelisk.Domain.Features.EmailTemplates.Entities;

namespace OneBeyond.Studio.Obelisk.Infrastructure.Data.Seeding;

internal sealed class EmailTemplatesSeeder : INotificationHandler<SeedApplication>
{
    private readonly DomainContext _domainContext;

    public EmailTemplatesSeeder(DomainContext domainContext)
        => _domainContext = EnsureArg.IsNotNull(domainContext, nameof(domainContext));

    public async Task HandleAsync(SeedApplication notification, CancellationToken cancellationToken = default)
    {
        foreach (var template in PredefinedEmailTemplates.All)
        {
            var templateExists = await _domainContext
                .Set<EmailTemplate>()
                .AnyAsync(
                    emailTemplate => emailTemplate.Id == template.Id,
                    cancellationToken);

            if (!templateExists)
            {
                _domainContext
                    .Set<EmailTemplate>()
                    .Add(template);
                await _domainContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
