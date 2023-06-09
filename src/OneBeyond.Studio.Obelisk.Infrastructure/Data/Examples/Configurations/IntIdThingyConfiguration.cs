using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneBeyond.Studio.DataAccess.EFCore.Configurations;
using OneBeyond.Studio.Obelisk.Domain.Features.Examples.Entities;

namespace OneBeyond.Studio.Obelisk.Infrastructure.Data.Examples.Configurations;

internal class IntIdThingyConfiguration : BaseEntityTypeConfiguration<IntIdThingy, int>
{
    protected override void DoConfigure(EntityTypeBuilder<IntIdThingy> builder)
    {
        base.DoConfigure(builder);

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();
    }
}
