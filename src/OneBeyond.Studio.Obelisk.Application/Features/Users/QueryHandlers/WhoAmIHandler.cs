using System;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using OneBeyond.Studio.Application.SharedKernel.AmbientContexts;
using OneBeyond.Studio.Application.SharedKernel.Repositories;
using OneBeyond.Studio.Core.Mediator;
using OneBeyond.Studio.Obelisk.Application.Features.Users.Dto;
using OneBeyond.Studio.Obelisk.Application.Features.Users.Queries;
using OneBeyond.Studio.Obelisk.Application.Services.AmbientContexts;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Entities;

namespace OneBeyond.Studio.Obelisk.Application.Features.Users.QueryHandlers;

internal sealed class WhoAmIHandler : IRequestHandler<WhoAmI, WhoAmIDto>
{
    private readonly IRORepository<UserBase, Guid> _userRORepository;
    private readonly UserContext _userContext;

    public WhoAmIHandler(
        IRORepository<UserBase, Guid> userRORepository,
        IAmbientContextAccessor<Services.AmbientContexts.AmbientContext> ambientContextAccessor)
    {
        EnsureArg.IsNotNull(userRORepository, nameof(userRORepository));
        EnsureArg.IsNotNull(ambientContextAccessor, nameof(ambientContextAccessor));

        _userRORepository = userRORepository;
        _userContext = ambientContextAccessor.AmbientContext.GetUserContext();
    }

    public async Task<WhoAmIDto> Handle(WhoAmI query, CancellationToken cancellationToken)
    {
        var user = await _userRORepository.GetByIdAsync<UserNameDto>(_userContext.UserId, cancellationToken).ConfigureAwait(false);

        return new WhoAmIDto
        {
            UserId = user.UserId,
            UserName = user.UserName,
            UserRoleId = _userContext.UserRoleId,
            UserTypeId = _userContext.UserTypeId
        };
    }
}
