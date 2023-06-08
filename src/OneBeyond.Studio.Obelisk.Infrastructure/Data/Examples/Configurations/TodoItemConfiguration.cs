using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneBeyond.Studio.DataAccess.EFCore.Configurations;
using OneBeyond.Studio.DataAccess.EFCore.RelationalTypeMappings;
using OneBeyond.Studio.Obelisk.Domain.Features.Examples;
using OneBeyond.Studio.Obelisk.Domain.Features.Examples.Entities;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Entities;

namespace OneBeyond.Studio.Obelisk.Infrastructure.Data.Examples.Configurations;

internal class TodoItemConfiguration: BaseEntityTypeConfiguration<TodoItem, Guid>
{
    protected override void DoConfigure(EntityTypeBuilder<TodoItem> builder)
    {
        base.DoConfigure(builder);

        builder.ToTable("MyTodoItems");

        builder.OwnsOne(x => x.Address, (addrBuilder) =>
        {
            addrBuilder.Property(x => x.HouseNo).HasColumnName("HouseNo");
            addrBuilder.Property(x => x.City);// Address_City //.HasColumnName("City"); //commented for testing purposes
            addrBuilder.Property(x => x.ZipCode).HasColumnName("Zip");
        });

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.AssignedToUserId);

        builder.HasMany(x => x.TodoItemProperties)
           .WithOne()
           .HasForeignKey(x => x.TodoItemId);

        builder
            .Property(x => x.Priority)
            .HasConversion(SmartEnumRelationalTypeMapping<TodoItemPriority, int>.Converter);
    }
}
