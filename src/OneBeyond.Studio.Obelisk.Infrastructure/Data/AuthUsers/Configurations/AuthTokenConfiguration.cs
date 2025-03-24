using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneBeyond.Studio.DataAccess.EFCore.Configurations;
using OneBeyond.Studio.Obelisk.Authentication.Application.Entities;

namespace OneBeyond.Studio.Obelisk.Infrastructure.Data.AuthUsers.Configurations;

internal sealed class AuthTokenConfiguration : BaseEntityTypeConfiguration<AuthToken, int>
{
    protected override void DoConfigure(EntityTypeBuilder<AuthToken> builder)
    {
        base.DoConfigure(builder);

        builder
            .Property(x => x.LoginId)
            .HasMaxLength(AuthUserConfiguration.LOGIN_ID_LENGTH)
            .IsRequired();

        builder
            .Property(x => x.RefreshToken)            
            .HasMaxLength(250)
            .IsRequired();

        builder.HasIndex(x => x.RefreshToken);        
    }
}
