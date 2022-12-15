using MediatR;
using OneBeyond.Studio.Obelisk.Authentication.Domain;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.Queries;

public sealed class GetExternalLoginInformation : IRequest<ExternalLoginInfo>
{
    public GetExternalLoginInformation()
    {
    }
}
