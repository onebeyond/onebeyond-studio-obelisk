using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OneBeyond.Studio.Application.SharedKernel.Exceptions;
using OneBeyond.Studio.Application.SharedKernel.Repositories.Exceptions;
using OneBeyond.Studio.Crosscuts.Exceptions;
using OneBeyond.Studio.Obelisk.Application.Exceptions;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Exceptions;
using OneBeyond.Studio.Obelisk.Authentication.Domain.JwtAuthentication.Exceptions;
using OneBeyond.Studio.Obelisk.Domain.Exceptions;

namespace OneBeyond.Studio.Obelisk.WebApi.ExceptionHandlers;

/// <summary>
/// Maps known domain and application exception types to appropriate HTTP status codes and ProblemDetails responses.
/// </summary>
internal sealed class DomainExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;

    public DomainExceptionHandler(IProblemDetailsService problemDetailsService)
    {
        ArgumentNullException.ThrowIfNull(problemDetailsService);
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Most-specific exception types must appear before their base types so the switch expression matches correctly.
        // In particular:
        //   - EntityNotFoundException, ObeliskDomainException, ObeliskApplicationException, and
        //     AuthorizationPolicyFailedException all extend OneBeyondException and must precede the OneBeyondException
        //     catch-all.
        //   - JwtAuthenticationFailedException extends AuthException and must precede the AuthException catch-all.
        var (statusCode, title, type) = exception switch
        {
            EntityNotFoundException => (
                StatusCodes.Status404NotFound,
                "Not Found",
                "https://tools.ietf.org/html/rfc9110#section-15.5.5"),

            ObeliskDomainException => (
                StatusCodes.Status400BadRequest,
                "Bad Request",
                "https://tools.ietf.org/html/rfc9110#section-15.5.1"),

            ObeliskApplicationException => (
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "https://tools.ietf.org/html/rfc9110#section-15.6.1"),

            AuthorizationPolicyFailedException => (
                StatusCodes.Status403Forbidden,
                "Forbidden",
                "https://tools.ietf.org/html/rfc9110#section-15.5.4"),

            // OneBeyondException catch-all for subtypes not matched above.
            OneBeyondException => (
                StatusCodes.Status400BadRequest,
                "Bad Request",
                "https://tools.ietf.org/html/rfc9110#section-15.5.1"),

            JwtAuthenticationFailedException => (
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                "https://tools.ietf.org/html/rfc9110#section-15.5.2"),

            // Auth-setup issues, not authentication failures — 400 is intentional.
            AuthException => (
                StatusCodes.Status400BadRequest,
                "Bad Request",
                "https://tools.ietf.org/html/rfc9110#section-15.5.1"),

            ArgumentException => (
                StatusCodes.Status400BadRequest,
                "Bad Request",
                "https://tools.ietf.org/html/rfc9110#section-15.5.1"),

            System.ComponentModel.DataAnnotations.ValidationException => (
                StatusCodes.Status400BadRequest,
                "Bad Request",
                "https://tools.ietf.org/html/rfc9110#section-15.5.1"),

            _ => default
        };

        if (statusCode is 0)
        {
            return false;
        }
        
        httpContext.Response.StatusCode = statusCode;

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Type = type,
                Title = title,
                Status = statusCode,
                Detail = exception.Message,
            }
        });
    }
}
