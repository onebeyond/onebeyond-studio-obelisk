using System;
using MediatR;
using OneBeyond.Studio.Obelisk.Domain.Features.Examples.Entities;

namespace OneBeyond.Studio.Obelisk.Domain.Features.Examples.Commands;

public sealed record DeleteTodoItem(Guid Id) : IRequest
{
}
 
