using Microsoft.AspNetCore.Identity;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.Schemes;

public sealed record IdentityApplicationScheme : IAuthenticationScheme
{
    public string SchemeName => IdentityConstants.ApplicationScheme;
}
