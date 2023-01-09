using System;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using OneBeyond.Studio.Application.SharedKernel.Repositories;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Commands;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Entities;

namespace OneBeyond.Studio.Obelisk.Application.Features.Users.CommandHandlers;

internal sealed class CreateUserHandler : IRequestHandler<CreateUser, Guid>
{
    private readonly IRWRepository<UserBase, Guid> _userRWRepository;

    public CreateUserHandler(IRWRepository<UserBase, Guid> userRWRepository)
    {
        EnsureArg.IsNotNull(userRWRepository, nameof(userRWRepository));

        _userRWRepository = userRWRepository;
    }

    public async Task<Guid> Handle(CreateUser command, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        var user = User.Apply(command);

        await _userRWRepository.CreateAsync(user, cancellationToken).ConfigureAwait(false);

        return user.Id;
    }
}
