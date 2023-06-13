using System;
using System.Collections.Generic;
using OneBeyond.Studio.Domain.SharedKernel.Entities;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Entities;

namespace OneBeyond.Studio.Obelisk.Domain.Features.Examples.Entities;

//NOTE! In case if we want to do bulk insert, bulk updatable properties MUST HAVE A PRIVATE SETTER

public abstract class TodoItemBase: AggregateRoot<Guid>
{
    protected readonly List<TodoItemProperty> _todoItemProperties = new();

#nullable disable
    protected TodoItemBase()
    {
    }
#nullable restore

    protected TodoItemBase(
        string title,
        Guid? assignedToUserId = null,
        DateTimeOffset? completedDate = null)
        : base(Guid.NewGuid())
    {
        Title = title;
        AssignedToUserId = assignedToUserId;
        CompletedDate = completedDate;
    }

    public string Title { get; protected set; }

    //[BulkUpdateExclude] This attribute can be used to exclude the property from bulk insert
    public Guid? AssignedToUserId { get; private set; }

    public User? AssignedToUser { get; private set; } //Navigation property

    public DateTimeOffset? CompletedDate { get; private set; }

    public bool IsComplete
        => CompletedDate.HasValue;

    public IReadOnlyCollection<TodoItemProperty> TodoItemProperties => _todoItemProperties.AsReadOnly();
}
