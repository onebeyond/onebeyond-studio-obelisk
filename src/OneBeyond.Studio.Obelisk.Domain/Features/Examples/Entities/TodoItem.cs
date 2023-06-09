using System;

namespace OneBeyond.Studio.Obelisk.Domain.Features.Examples.Entities;

//NOTE! In case if we want to do bulk insert, bulk updatable properties MUST HAVE A PRIVATE SETTER

public sealed class TodoItem: TodoItemBase
{ 

#nullable disable
    private TodoItem() 
    {
    }
#nullable restore

    public TodoItem(
        string title,
        TodoItemPriority priority,
        TodoAddress? address = null,
        Guid? assignedToUserId = null,
        DateTimeOffset? completedDate = null)
        : base(title, assignedToUserId, completedDate)
    {
        Priority = priority;
        Address = address;
    }

    public TodoItemPriority Priority { get; private set; }

    public TodoAddress? Address { get; private set; }
}
