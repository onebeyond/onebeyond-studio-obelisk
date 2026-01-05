using OneBeyond.Studio.Core.Mediator.Queries;
using OneBeyond.Studio.Obelisk.Application.Features.Users.Dto;

namespace OneBeyond.Studio.Obelisk.Application.Features.Users.Queries;

public sealed record WhoAmI : IQuery<WhoAmIDto>
{
}
