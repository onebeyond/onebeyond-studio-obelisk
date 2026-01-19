using OneBeyond.Studio.Core.Mediator;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.Queries;

public sealed class GetExternalLoginInformation : IRequest<ExternalLoginInfo>
{
    public GetExternalLoginInformation()
    {
    }
}
