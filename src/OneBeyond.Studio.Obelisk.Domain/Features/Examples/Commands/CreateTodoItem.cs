using System;
using MediatR;
using OneBeyond.Studio.Obelisk.Domain.Features.Examples.Entities;

namespace OneBeyond.Studio.Obelisk.Domain.Features.Examples.Commands;

public sealed record CreateTodoItem : IRequest<Guid>
{
    public CreateTodoItem(
        string title,
        int priority,
        TodoAddress address
        )
    {
        Title = title;
        Address = address;
        Priority = TodoItemPriority.FromValue(priority);
    }

    public string Title { get; init; }
    public TodoItemPriority Priority { get; init; } 
    public TodoAddress Address { get; init; }
}
 
