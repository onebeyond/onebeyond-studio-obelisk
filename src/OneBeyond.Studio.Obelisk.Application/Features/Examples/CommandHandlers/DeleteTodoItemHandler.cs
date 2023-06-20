using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OneBeyond.Studio.Application.SharedKernel.Repositories;
using OneBeyond.Studio.Obelisk.Domain.Features.Examples.Commands;
using OneBeyond.Studio.Obelisk.Domain.Features.Examples.Entities;

namespace OneBeyond.Studio.Obelisk.Application.Features.Examples.CommandHandlers;

internal sealed class DeleteTodoItemHandler : IRequestHandler<DeleteTodoItem>
{
    private readonly IRWRepository<TodoItem, Guid> _repository;

    public DeleteTodoItemHandler(IRWRepository<TodoItem, Guid> repository)
    {
        _repository = repository;
    }

    public  Task Handle(DeleteTodoItem request, CancellationToken cancellationToken)
    {
        return _repository.DeleteAsync(request.Id, cancellationToken);
    }
}

