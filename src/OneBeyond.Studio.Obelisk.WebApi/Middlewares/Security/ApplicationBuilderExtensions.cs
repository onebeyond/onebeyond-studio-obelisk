using Microsoft.AspNetCore.Builder;

namespace OneBeyond.Studio.Obelisk.WebApi.Middlewares.Security;

internal static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseSecurityHeadersMiddleware(this IApplicationBuilder app, SecurityHeadersBuilder builder)
    {
        var policy = builder.Build();
        return app.UseMiddleware<SecurityHeadersMiddleware>(policy);
    }
}
