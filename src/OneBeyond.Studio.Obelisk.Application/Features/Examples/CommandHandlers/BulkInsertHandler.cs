using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using OneBeyond.Studio.Application.SharedKernel.Repositories;
using OneBeyond.Studio.Obelisk.Application.Repositories;
using OneBeyond.Studio.Obelisk.Domain.Features.Examples;
using OneBeyond.Studio.Obelisk.Domain.Features.Examples.Commands;
using OneBeyond.Studio.Obelisk.Domain.Features.Examples.Entities;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Commands;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Entities;

namespace OneBeyond.Studio.Obelisk.Application.Features.Examples.CommandHandlers;

internal sealed class BulkInsertHandler : IRequestHandler<BulkInsert>
{
    private readonly IRORepository<User, Guid> _userRORepository;
    private readonly IRWBulkRepository<TodoItem, Guid> _todoRWRepository;

    public BulkInsertHandler(
        IRORepository<User, Guid> userRORepository,
        IRWBulkRepository<TodoItem, Guid> todoRWRepository)
    {
        _userRORepository = EnsureArg.IsNotNull(userRORepository, nameof(userRORepository));
        _todoRWRepository = EnsureArg.IsNotNull(todoRWRepository, nameof(todoRWRepository));
    }

    public async Task Handle(BulkInsert command, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        var rnd = new Random();

        //It's a test code, we do know there is one user in the database
        var users = await _userRORepository.ListAsync(u => u.RoleId == UserRole.ADMINISTRATOR, cancellationToken: cancellationToken).ConfigureAwait(false);
        var userId = users.First().Id;

        var todoitems = Enumerable
            .Range(1, command.Count)
            .Select(x => new TodoItem(
                $"TodoItem {x}",
                TodoItemPriority.FromValue(rnd.Next(2)), 
                new TodoAddress(
                    rnd.Next(100),
                    $"City  {x}",
                    $"Zip{x}"),
                userId, DateTimeOffset.UtcNow));

        await _todoRWRepository.BulkInsertAsync(todoitems, cancellationToken).ConfigureAwait(false);
    }
}
