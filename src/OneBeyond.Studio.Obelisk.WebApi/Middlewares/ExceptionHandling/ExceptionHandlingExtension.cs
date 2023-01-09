using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using OneBeyond.Studio.Application.SharedKernel.Exceptions;
using OneBeyond.Studio.Obelisk.Application.Exceptions;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Exceptions;
using OneBeyond.Studio.Obelisk.Authentication.Domain.JwtAuthentication.Exceptions;
using OneBeyond.Studio.Obelisk.Domain.Exceptions;

namespace OneBeyond.Studio.Obelisk.WebApi.Middlewares.ExceptionHandling;

internal static class ExceptionHandlingExtension
{
    public static void UseExceptionHandling(this IApplicationBuilder applicationBuilder)
        => applicationBuilder.Use(HandleDomainException);

    private static async Task HandleDomainException(HttpContext httpContext, Func<Task> next)
    {
        try
        {
            await next();
        }
        catch (ObeliskDomainException domainException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(domainException.Message));
        }
        catch (ObeliskApplicationException applicationException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await httpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(applicationException.Message));
        }
        catch (JwtException jwtException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await httpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(jwtException.Message));
        }
        catch (AuthException authnException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await httpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(authnException.Message));
        }
        catch (AuthorizationPolicyFailedException authzException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            await httpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(authzException.Message));
        }
    }
}
