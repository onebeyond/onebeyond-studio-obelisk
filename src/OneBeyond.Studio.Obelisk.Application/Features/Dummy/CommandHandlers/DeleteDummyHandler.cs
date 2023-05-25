using System;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using OneBeyond.Studio.Application.SharedKernel.Repositories;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Exceptions;
using OneBeyond.Studio.Obelisk.Domain.Exceptions;
using OneBeyond.Studio.Obelisk.Domain.Features.Dummies.Commands;
using OneBeyond.Studio.Obelisk.Domain.Features.Dummies.Entities;

namespace OneBeyond.Studio.Obelisk.Application.Features.Dummies.CommandHandlers;

internal sealed class DeleteDummyHandler : IRequestHandler<DeleteDummy>
{
    private readonly IRWRepository<Dummy, Guid> _dummyRWRepository;
    private readonly IMediator _mediator;

    public DeleteDummyHandler(
        IRWRepository<Dummy, Guid> dummyRWRepository,
        IMediator mediator)
    {
        EnsureArg.IsNotNull(dummyRWRepository, nameof(dummyRWRepository));
        EnsureArg.IsNotNull(mediator, nameof(mediator));

        _dummyRWRepository = dummyRWRepository;
        _mediator = mediator;
    }

    public async Task Handle(DeleteDummy command, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        var dummy = await _dummyRWRepository.GetByIdAsync(command.Id, cancellationToken: cancellationToken).ConfigureAwait(false);

        await _dummyRWRepository.DeleteAsync(dummy, cancellationToken).ConfigureAwait(false);
    }
}
