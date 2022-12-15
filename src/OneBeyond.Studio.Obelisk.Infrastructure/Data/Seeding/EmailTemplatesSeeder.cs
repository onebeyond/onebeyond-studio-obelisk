using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneBeyond.Studio.Obelisk.Application.Services.Seeding;
using OneBeyond.Studio.Obelisk.Domain.Features.EmailTemplates.Entities;

namespace OneBeyond.Studio.Obelisk.Infrastructure.Data.Seeding;

internal sealed class EmailTemplatesSeeder : INotificationHandler<SeedApplication>
{
    private readonly DomainContext _domainContext;

    public EmailTemplatesSeeder(DomainContext domainContext)
    {
        EnsureArg.IsNotNull(domainContext, nameof(domainContext));

        _domainContext = domainContext;
    }

    public async Task Handle(SeedApplication notification, CancellationToken cancellationToken)
    {
        await SeedPredefinedEmailTemplateAsync(
                _domainContext,
                PredefinedEmailTemplates.DefaultAccountSetupEmailTemplate,
                cancellationToken)
            .ConfigureAwait(false);
        await SeedPredefinedEmailTemplateAsync(
                _domainContext,
                PredefinedEmailTemplates.DefaultPasswordResetEmailTemplate,
                cancellationToken)
            .ConfigureAwait(false);
    }

    private static async Task SeedPredefinedEmailTemplateAsync(
        DomainContext context,
        EmailTemplate template,
        CancellationToken cancellationToken)
    {
        var templateExists = await context
            .Set<EmailTemplate>()
            .AnyAsync(
                x => x.Id == template.Id,
                cancellationToken)
            .ConfigureAwait(false);

        if (!templateExists)
        {
            context
                .Set<EmailTemplate>()
                .Add(template);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
