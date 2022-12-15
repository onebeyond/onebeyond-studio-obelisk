using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.Schemes;

public sealed record JWTScheme : IAuthenticationScheme
{
    public string SchemeName => JwtBearerDefaults.AuthenticationScheme;
}
