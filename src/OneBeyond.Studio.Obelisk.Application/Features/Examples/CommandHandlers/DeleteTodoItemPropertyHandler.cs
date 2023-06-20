using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OneBeyond.Studio.Application.SharedKernel.Repositories;
using OneBeyond.Studio.Application.SharedKernel.Specifications;
using OneBeyond.Studio.Obelisk.Domain.Features.Examples.Commands;
using OneBeyond.Studio.Obelisk.Domain.Features.Examples.Entities;

namespace OneBeyond.Studio.Obelisk.Application.Features.Examples.CommandHandlers;

internal sealed class DeleteTodoItemPropertyHandler : IRequestHandler<DeleteTodoItemProperty>
{
    private readonly IRWRepository<TodoItem, Guid> _repository;

    public DeleteTodoItemPropertyHandler(IRWRepository<TodoItem, Guid> repository)
    {
        _repository = repository;
    }

    public async Task Handle(DeleteTodoItemProperty request, CancellationToken cancellationToken)
    {
        var todoItem = await _repository.GetByIdAsync(
            request.TodoItemId,
            Includes.Create<TodoItem>().Include(x => x.TodoItemProperties),
            cancellationToken);

        todoItem.DeleteProperty(request.PropertyId);

        await _repository.UpdateAsync(todoItem, cancellationToken);
    }
}
