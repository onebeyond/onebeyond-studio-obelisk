using OneBeyond.Studio.DataAccess.EFCore.Configurations;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Entities;

namespace OneBeyond.Studio.Obelisk.Infrastructure.Data.Users.Configurations;

internal sealed class UserConfiguration : DerivedEntityTypeConfiguration<User, UserBase>
{
}
