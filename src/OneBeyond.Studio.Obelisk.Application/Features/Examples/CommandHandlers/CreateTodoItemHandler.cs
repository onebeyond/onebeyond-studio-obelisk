using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OneBeyond.Studio.Application.SharedKernel.Repositories;
using OneBeyond.Studio.Obelisk.Domain.Features.Examples.Commands;
using OneBeyond.Studio.Obelisk.Domain.Features.Examples.Entities;

namespace OneBeyond.Studio.Obelisk.Application.Features.Examples.CommandHandlers;

internal sealed class CreateTodoItemHandler: IRequestHandler<CreateTodoItem, Guid>
{
    private readonly IRWRepository<TodoItem, Guid> _repository;

    public CreateTodoItemHandler(IRWRepository<TodoItem, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateTodoItem request, CancellationToken cancellationToken)
    {
        var todoItem = new TodoItem(request.Title, request.Priority, request.Address);
        await _repository.CreateAsync(todoItem, cancellationToken);
        return todoItem.Id;
    }
}
