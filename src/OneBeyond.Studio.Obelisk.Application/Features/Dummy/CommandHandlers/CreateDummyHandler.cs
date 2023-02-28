using System;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using OneBeyond.Studio.Application.SharedKernel.Repositories;
using OneBeyond.Studio.Obelisk.Domain.Features.Dummies.Commands;
using OneBeyond.Studio.Obelisk.Domain.Features.Dummies.Entities;

namespace OneBeyond.Studio.Obelisk.Application.Features.Dummies.CommandHandlers;

internal sealed class CreateDummyHandler : IRequestHandler<CreateDummy, Guid>
{
    private readonly IRWRepository<Dummy, Guid> _DummyRWRepository;

    public CreateDummyHandler(IRWRepository<Dummy, Guid> DummyRWRepository)
    {
        EnsureArg.IsNotNull(DummyRWRepository, nameof(DummyRWRepository));

        _DummyRWRepository = DummyRWRepository;
    }

    public async Task<Guid> Handle(CreateDummy command, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        var dummy = Dummy.Apply(command);

        await _DummyRWRepository.CreateAsync(dummy, cancellationToken).ConfigureAwait(false);

        return dummy.Id;
    }
}
