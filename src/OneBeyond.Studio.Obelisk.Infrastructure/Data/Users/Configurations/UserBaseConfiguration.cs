using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneBeyond.Studio.DataAccess.EFCore.Configurations;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Entities;
using OneBeyond.Studio.Obelisk.Infrastructure.Data.AuthUsers.Configurations;

namespace OneBeyond.Studio.Obelisk.Infrastructure.Data.Users.Configurations;

internal sealed class UserBaseConfiguration : BaseEntityTypeConfiguration<UserBase, Guid>
{
    protected override void DoConfigure(EntityTypeBuilder<UserBase> builder)
    {
        base.DoConfigure(builder);

        builder
            .ToTable("Users");

        builder
            .Property(x => x.TypeId)
            .IsRequired()
            .HasMaxLength(150);

        builder
            .Property(x => x.UserName)
            .IsRequired()
            .HasMaxLength(450);

        builder
            .Property(x => x.RoleId)
            .HasMaxLength(450);

        builder
            .Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(450);

        builder
            .Property(x => x.LoginId)
            .HasMaxLength(AuthUserConfiguration.LOGIN_ID_LENGTH)
            .IsRequired();
    }
}
