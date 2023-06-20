using System;
using MediatR;
using OneBeyond.Studio.Obelisk.Domain.Features.Examples.Entities;

namespace OneBeyond.Studio.Obelisk.Domain.Features.Examples.Commands;

public sealed record UpdateTodoItemProperty(Guid TodoItemId, Guid PropertyId, string Value) : IRequest
{
}
 
