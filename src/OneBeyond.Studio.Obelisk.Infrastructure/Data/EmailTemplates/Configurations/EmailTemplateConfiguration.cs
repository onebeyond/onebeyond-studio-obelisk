using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneBeyond.Studio.DataAccess.EFCore.Configurations;
using OneBeyond.Studio.Obelisk.Domain.Features.EmailTemplates.Entities;

namespace OneBeyond.Studio.Obelisk.Infrastructure.Data.EmailTemplates.Configurations;

internal sealed class EmailTemplateConfiguration : BaseEntityTypeConfiguration<EmailTemplate, string>
{
    protected override void DoConfigure(EntityTypeBuilder<EmailTemplate> builder)
    {
        builder.Property(x => x.Id)
            .IsRequired()
            .HasMaxLength(100);
    }
}
