using System;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using OneBeyond.Studio.Application.SharedKernel.Repositories;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Exceptions;
using OneBeyond.Studio.Obelisk.Domain.Exceptions;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Commands;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Entities;

namespace OneBeyond.Studio.Obelisk.Application.Features.Users.CommandHandlers;

internal sealed class UnlockUserHandler : IRequestHandler<UnlockUser>
{
    private readonly IRWRepository<UserBase, Guid> _userRWRepository;
    private readonly IMediator _mediator;

    public UnlockUserHandler(
        IRWRepository<UserBase, Guid> userRWRepository,
        IMediator mediator)
    {
        EnsureArg.IsNotNull(userRWRepository, nameof(userRWRepository));
        EnsureArg.IsNotNull(mediator, nameof(mediator));

        _userRWRepository = userRWRepository;
        _mediator = mediator;
    }

    public async Task<Unit> Handle(
        UnlockUser command,
        CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        var user = await _userRWRepository.GetByIdAsync(command.UserId, cancellationToken).ConfigureAwait(false);

        try
        {
            await _mediator.Send(
                new UnlockLogin(user.LoginId),
                cancellationToken).ConfigureAwait(false);
        }
        catch (AuthException authException)
        {
            throw new ObeliskDomainException(authException.Message);
        }

        user.Unlock();

        await _userRWRepository.UpdateAsync(user, cancellationToken).ConfigureAwait(false);

        return Unit.Value;
    }

}
