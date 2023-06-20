using System;
using OneBeyond.Studio.DataAccess.EFCore.Configurations;
using OneBeyond.Studio.Obelisk.Domain.Features.Examples.Entities;

namespace OneBeyond.Studio.Obelisk.Infrastructure.Data.Examples.Configurations;

internal sealed class TodoItemPropertyConfiguration : BaseEntityTypeConfiguration<TodoItemProperty, Guid>
{
}
