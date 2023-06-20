using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OneBeyond.Studio.Application.SharedKernel.Repositories;
using OneBeyond.Studio.Obelisk.Domain.Features.Examples.Commands;
using OneBeyond.Studio.Obelisk.Domain.Features.Examples.Entities;

namespace OneBeyond.Studio.Obelisk.Application.Features.Examples.CommandHandlers;

internal sealed class CompleteTodoItemHandler : IRequestHandler<CompleteTodoItem>
{
    private readonly IRWRepository<TodoItem, Guid> _repository;

    public CompleteTodoItemHandler(IRWRepository<TodoItem, Guid> repository)
    {
        _repository = repository;
    }

    public async Task Handle(CompleteTodoItem request, CancellationToken cancellationToken)
    {
        var todoItem = await _repository.GetByIdAsync(request.Id, cancellationToken);

        todoItem.Complete();

        await _repository.UpdateAsync(todoItem, cancellationToken);
    }
}

