using System;
using System.Collections.Generic;
using System.Linq;
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

    public Guid? AssignedToUserId { get; private set; }

    public User? AssignedToUser { get; private set; } //Navigation property

    public DateTimeOffset? CompletedDate { get; private set; }

    public bool IsComplete
        => CompletedDate.HasValue;

    public IReadOnlyCollection<TodoItemProperty> TodoItemProperties => _todoItemProperties.AsReadOnly();

    public void Assign(Guid? userId)
        => AssignedToUserId = userId;

    public void Complete() => CompletedDate = DateTimeOffset.UtcNow;

    public TodoItemProperty AddProperty(string name, string value)
    {
        var prop = new TodoItemProperty(Id, name, value);
        _todoItemProperties.Add(prop);
        return prop;
    }

    public void UpdateProperty(Guid id, string value)
        => _todoItemProperties.FirstOrDefault(x => x.Id == id)?.Update(value);

    public void DeleteProperty(Guid id)
    {
        var prop = _todoItemProperties.FirstOrDefault(x => x.Id == id);

        if (prop is not null)
        {
            _todoItemProperties.Remove(prop);
        }
    }
}
