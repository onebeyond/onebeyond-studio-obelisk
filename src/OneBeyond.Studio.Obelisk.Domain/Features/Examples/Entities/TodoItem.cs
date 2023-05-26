using System;
using System.Collections.Generic;
using OneBeyond.Studio.Domain.SharedKernel.Entities;

namespace OneBeyond.Studio.Obelisk.Domain.Features.Examples.Entities;

//NOTE! In case if we want to do bulk insert, bulk updatable properties MUST HAVE A PRIVATE SETTER

//[BulkUpdateTableName("TodoItems")] Use this attribute if your table has a custom name
public sealed class TodoItem: AggregateRoot<Guid>
{
    private readonly List<TodoItemProperty> _todoItemProperties = new();

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
        CompletedDate = completedDate;
    }

    public string Title { get; private set; }

    //TODO To implement
    //public TodoItemPriority Priority { get; private set; }

    public TodoAddress? Address { get; private set; }

    //[BulkUpdateExclude] This attribute can be used to exclude the property from bulk insert
    public Guid? AssignedToUserId { get; private set; }

    public DateTimeOffset? CompletedDate { get; private set; }

    public bool IsComplete
        => CompletedDate.HasValue;

    public IReadOnlyCollection<TodoItemProperty> TodoItemProperties => _todoItemProperties.AsReadOnly();
}
