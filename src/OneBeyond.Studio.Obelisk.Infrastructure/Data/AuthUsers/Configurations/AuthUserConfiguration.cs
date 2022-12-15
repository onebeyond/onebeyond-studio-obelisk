using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneBeyond.Studio.Obelisk.Authentication.Application.Entities;

namespace OneBeyond.Studio.Obelisk.Infrastructure.Data.AuthUsers.Configurations;

internal sealed class AuthUserConfiguration : IEntityTypeConfiguration<AuthUser>
{
    public const int LOGIN_ID_LENGTH = 450;

    public void Configure(EntityTypeBuilder<AuthUser> builder)
    {
        builder
            .ToTable("AspNetUsers");
        builder
            .HasKey((e) => e.Id);
        builder
            .Property((e) => e.Id)
            .ValueGeneratedOnAdd();

        builder
            .HasMany(x => x.AuthTokens) //OwnsMany does not work with UserManager, that's why we use AuthUserStore
            .WithOne()
            .HasForeignKey(x => x.LoginId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
