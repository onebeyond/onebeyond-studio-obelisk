using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace OneBeyond.Studio.Obelisk.WebApi.Middlewares.Security;

internal sealed class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly SecurityHeadersPolicy _policy;

    public SecurityHeadersMiddleware(RequestDelegate next, SecurityHeadersPolicy policy)
    {
        _next = next;
        _policy = policy;
    }

    public async Task Invoke(HttpContext context)
    {
        var headers = context.Response.Headers;

        foreach (var headerValuePair in _policy.SetHeaders)
        {
            headers[headerValuePair.Key] = headerValuePair.Value;
        }

        foreach (var header in _policy.RemoveHeaders)
        {
            headers.Remove(header);
        }

        await _next(context);
    }
}
