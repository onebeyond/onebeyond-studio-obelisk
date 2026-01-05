using System.Linq;
using OneBeyond.Studio.DataAccess.EFCore.Projections;
using OneBeyond.Studio.Obelisk.Application.Services.EmailTemplateService;
using OneBeyond.Studio.Obelisk.Domain.Features.EmailTemplates.Entities;

namespace OneBeyond.Studio.Obelisk.Infrastructure.Data.EmailTemplates.Projections;

public class EmailTemplateProjection : IEntityTypeProjection<EmailTemplate, EmailTemplateDto>
{
    public IQueryable<EmailTemplateDto> Project(IQueryable<EmailTemplate> entityQuery, ProjectionContext context)
    {
        return entityQuery.Select(emailTemplate => new EmailTemplateDto
        {
            Subject = emailTemplate.Subject,
            Body = emailTemplate.Body
        });
    }
}
