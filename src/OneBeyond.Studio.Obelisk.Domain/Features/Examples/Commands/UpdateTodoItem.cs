using System;
using MediatR;
using OneBeyond.Studio.Obelisk.Domain.Features.Examples.Entities;

namespace OneBeyond.Studio.Obelisk.Domain.Features.Examples.Commands;

public sealed record UpdateTodoItem : IRequest
{
    public UpdateTodoItem(
        Guid id,
        string title,
        int priority,
        TodoAddress address
        )
    {
        Id = id;
        Title = title;
        Address = address;
        Priority = TodoItemPriority.FromValue(priority);
    }

    public Guid Id { get; }
    public string Title { get; init; }
    public TodoItemPriority Priority { get; init; }
    public TodoAddress Address { get; init; }
}
 
