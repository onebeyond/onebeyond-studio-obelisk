using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneBeyond.Studio.DataAccess.EFCore.Configurations;
using OneBeyond.Studio.Obelisk.Domain.Features.Audit.Entities;

namespace OneBeyond.Studio.Obelisk.Infrastructure.Data.Audit.Configurations;
internal sealed class AuditRecordConfiguration : BaseEntityTypeConfiguration<AuditRecord, int>
{
    protected override void DoConfigure(EntityTypeBuilder<AuditRecord> builder)
    {
        base.DoConfigure(builder);

        builder.Property(x => x.Date);
        builder.Property(x => x.UserId);
        builder.Property(x => x.EntityId);
        builder.Property(x => x.EntityType);
        builder.Property(x => x.ChangeType);
        builder.Property(x => x.Changes);
        builder.Property(x => x.Children);
    }
}
