using MediatR;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.Queries;

public sealed class GetExternalLoginInformation : IRequest<ExternalLoginInfo>
{
    public GetExternalLoginInformation()
    {
    }
}
