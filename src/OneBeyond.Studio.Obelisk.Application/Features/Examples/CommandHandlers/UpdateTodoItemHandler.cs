using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OneBeyond.Studio.Application.SharedKernel.Repositories;
using OneBeyond.Studio.Obelisk.Domain.Features.Examples.Commands;
using OneBeyond.Studio.Obelisk.Domain.Features.Examples.Entities;

namespace OneBeyond.Studio.Obelisk.Application.Features.Examples.CommandHandlers;

internal sealed class UpdateTodoItemHandler : IRequestHandler<UpdateTodoItem>
{
    private readonly IRWRepository<TodoItem, Guid> _repository;

    public UpdateTodoItemHandler(IRWRepository<TodoItem, Guid> repository)
    {
        _repository = repository;
    }

    public async Task Handle(UpdateTodoItem request, CancellationToken cancellationToken)
    {
        var todoItem = await _repository.GetByIdAsync(request.Id, cancellationToken);

        todoItem.Update(request);

        await _repository.UpdateAsync(todoItem, cancellationToken);
    }
}

