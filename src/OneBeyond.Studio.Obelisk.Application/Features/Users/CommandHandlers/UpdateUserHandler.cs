using System;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using OneBeyond.Studio.Application.SharedKernel.Repositories;
using OneBeyond.Studio.Obelisk.Authentication.Domain;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Commands;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Entities;

namespace OneBeyond.Studio.Obelisk.Application.Features.Users.CommandHandlers;

internal sealed class UpdateUserHandler : IRequestHandler<UpdateUser, UpdateUserResult>
{
    private readonly IRWRepository<UserBase, Guid> _userRWRepository;
    private readonly IMediator _mediator;

    public UpdateUserHandler(
        IRWRepository<UserBase, Guid> userRWRepository,
        IMediator mediator)
    {
        EnsureArg.IsNotNull(userRWRepository, nameof(userRWRepository));
        EnsureArg.IsNotNull(mediator, nameof(mediator));

        _userRWRepository = userRWRepository;
        _mediator = mediator;
    }

    public async Task<UpdateUserResult> Handle(UpdateUser command, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        var user = await _userRWRepository.GetByIdAsync(command.UserId, cancellationToken: cancellationToken).ConfigureAwait(false);

        //Please note! The reason we use mediator within a command handler
        //is because we consider Authentication project as an external for us (for our Domain).
        //In case if you want to use mediator to execute commands or your Domain - that most likely would be considered as a code smell.
        var result = await _mediator.Send(
            new UpdateLogin(
                user.LoginId,
                command.UserName,
                command.Email,
                command.RoleId
            ),
            cancellationToken).ConfigureAwait(false);

        if (result.Success)
        {
            user.Apply(command);

            await _userRWRepository.UpdateAsync(user, cancellationToken).ConfigureAwait(false);
        }
        return new UpdateUserResult(result.Success, result.Errors);
    }
}
