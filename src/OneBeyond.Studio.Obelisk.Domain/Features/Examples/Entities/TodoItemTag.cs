using System;
using OneBeyond.Studio.Domain.SharedKernel.Entities;

namespace OneBeyond.Studio.Obelisk.Domain.Features.Examples.Entities;

public sealed class TodoItemTag : DomainEntity<int>
{
#nullable disable
    private TodoItemTag()
    {
    }
#nullable restore

    internal TodoItemTag(
        Guid todoItemId,
        string name)
        : base()
    {
        TodoItemId = todoItemId;
        Name = name;
    }

    public Guid TodoItemId { get; }
    public string Name { get; private set; }
}
