using System;
using OneBeyond.Studio.Domain.SharedKernel.Entities;

namespace OneBeyond.Studio.Obelisk.Domain.Features.Examples.Entities;

public sealed class TodoItemProperty : DomainEntity<Guid>
{
#nullable disable
    private TodoItemProperty()
    {
    }
#nullable restore

    internal TodoItemProperty(
        Guid todoItemId,
        string name,
        string value)
        : base(Guid.NewGuid())
    {
        TodoItemId = todoItemId;
        Name = name;
        Value = value;
    }

    public Guid TodoItemId { get; }
    public string Name { get; private set; }
    public string Value { get; private set; }
}
