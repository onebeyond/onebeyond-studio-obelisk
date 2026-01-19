using OneBeyond.Studio.Core.Mediator;
using OneBeyond.Studio.Obelisk.Application.Features.Users.Dto;

namespace OneBeyond.Studio.Obelisk.Application.Features.Users.Queries;

public sealed record WhoAmI : IRequest<WhoAmIDto>
{
}
