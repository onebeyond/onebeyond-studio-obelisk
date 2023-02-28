using System;
using Castle.Core.Resource;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneBeyond.Studio.DataAccess.EFCore.Configurations;
using OneBeyond.Studio.Obelisk.Domain.Features.Dummies.Entities;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Entities;
using OneBeyond.Studio.Obelisk.Infrastructure.Data.AuthUsers.Configurations;

namespace OneBeyond.Studio.Obelisk.Infrastructure.Data.Users.Configurations;

internal sealed class DummyConfiguration : BaseEntityTypeConfiguration<Dummy, Guid>
{
    protected override void DoConfigure(EntityTypeBuilder<Dummy> builder)
    {
        base.DoConfigure(builder);

        builder
            .ToTable("Dummies");

        builder
            .Property(x => x.StringValue)
            .HasMaxLength(150);

        builder
            .Property(x => x.NumericValue)
            .HasColumnType("decimal(6, 2)");

        builder
            .Property(x => x.BoolValue);

        builder.Property(x => x.DateTimeValue);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.FKValueId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
