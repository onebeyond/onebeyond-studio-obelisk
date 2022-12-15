using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneBeyond.Studio.DataAccess.EFCore.Configurations;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Entities;

namespace OneBeyond.Studio.Obelisk.Infrastructure.Data.Users.Configurations;

internal sealed class UserLoginLogConfiguration : BaseEntityTypeConfiguration<UserLoginLog, int>
{
    protected override void DoConfigure(EntityTypeBuilder<UserLoginLog> builder)
    {
        base.DoConfigure(builder);

        builder.Property(x => x.Username)
            .HasMaxLength(450);

        builder.Property(x => x.Status)
             .HasMaxLength(1000);
    }
}
