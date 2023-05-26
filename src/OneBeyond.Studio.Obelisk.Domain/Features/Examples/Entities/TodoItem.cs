using System;
using OneBeyond.Studio.Domain.SharedKernel.Entities;
using OneBeyond.Studio.Obelisk.Domain.Attributes;
using OneBeyond.Studio.Obelisk.Domain.Features.Examples.Entities;

namespace OneBeyond.Studio.Obelisk.Domain.Features.Examples;

//NOTE! In case if we want to do bulk insert, bulk updatable properties must have a private setter

//[BulkUpdateTableName("TodoItems")]
public sealed class TodoItem: AggregateRoot<Guid>
{

#nullable disable
    private TodoItem()
    {
    }
#nullable restore

    public TodoItem(
        string title,
        //TodoItemPriority priority,
        TodoAddress? address = null,
        Guid? assignedToUserId = null,
        DateTimeOffset? completedDate = null)
        : base(Guid.NewGuid())
    {
        Title = title;
        //Priority = priority;
        Address = address;
        AssignedToUserId = assignedToUserId;
        CompletiedDate = completedDate;
    }

    public string Title { get; private set; }

    //TODO To implement
    //public TodoItemPriority Priority { get; private set; }

    public TodoAddress? Address { get; private set; }

    //[BulkUpdateExclude] This attribute can be used to exclude the property from bulk insert
    public Guid? AssignedToUserId { get; private set; }

    public DateTimeOffset? CompletiedDate { get; private set; }

    public bool IsComplete
        => CompletiedDate.HasValue;
}
