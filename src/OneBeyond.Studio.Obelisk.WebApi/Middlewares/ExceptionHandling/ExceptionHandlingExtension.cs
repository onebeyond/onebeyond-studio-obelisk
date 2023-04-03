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
        catch (JwtAuthenticationFailedException jwtException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await httpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(jwtException.Message));
        }
        catch (AuthException authnException)
        {
            //Those are actually not "Authentication failed" exceptions. 
            //Those are "Those are issues with setting up authentication mechanism properly" exceptions.
            //That's why we do NOT consider them as "Status401Unauthorized".
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(authnException.Message));
        }
        catch (ArgumentException argumentException) 
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(argumentException.Message));
        }
        catch (AuthorizationPolicyFailedException authzException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            await httpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(authzException.Message));
        }
    }
}
