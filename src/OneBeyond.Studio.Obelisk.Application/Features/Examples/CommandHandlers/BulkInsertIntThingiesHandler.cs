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
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Entities;

namespace OneBeyond.Studio.Obelisk.Application.Features.Examples.CommandHandlers;

internal sealed class BulkInsertIntThingiesHandler : IRequestHandler<BulkInsertIntThingies>
{
    private readonly IRWBulkRepository<IntIdThingy, int> _thingyRWRepository;

    public BulkInsertIntThingiesHandler(
        IRWBulkRepository<IntIdThingy, int> thingyRWRepository)
    {
        _thingyRWRepository = EnsureArg.IsNotNull(thingyRWRepository, nameof(thingyRWRepository));
    }

    public async Task Handle(BulkInsertIntThingies command, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        var rnd = new Random();

        //It's a test code, we do know there is one user in the database

        var thingies = Enumerable
            .Range(1, command.Count)
            .Select(x => new IntIdThingy(
                $"Thingy {x}",
                rnd.Next(10) < 5));

        await _thingyRWRepository.BulkInsertAsync(thingies, cancellationToken).ConfigureAwait(false);
    }
}
