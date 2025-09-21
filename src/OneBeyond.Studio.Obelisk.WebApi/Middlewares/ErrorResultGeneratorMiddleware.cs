using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OneBeyond.Studio.Application.SharedKernel.Repositories.Exceptions;
using OneBeyond.Studio.Crosscuts.Exceptions;

namespace OneBeyond.Studio.Obelisk.WebApi.Middlewares;

internal sealed class ErrorResultGeneratorMiddleware
{
    public const string ProblemContent = "application/problem+json";
    public const string TraceIdKey = "traceId";

    private const string ErrorMessageTemplate = "Error has occurred while processing HTTP request.{CheckErrorResponse}";
    private const string CheckResponseMessage = " Check error response";

    private readonly ILogger<ErrorResultGeneratorMiddleware> _logger;
    private readonly RequestDelegate _next;

    public ErrorResultGeneratorMiddleware(ILogger<ErrorResultGeneratorMiddleware> logger, RequestDelegate next)
    {
        _logger = logger;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        //NB: This catch can be removed when the new SharedKernel package will be available
        catch (ValidationException exception)
        {
            var result = new BadRequestObjectResult(exception.Message);

            await result.ExecuteResultAsync(new ActionContext
            {
                HttpContext = httpContext
            });
        }
        catch (OneBeyondException exception) when (!httpContext.Response.HasStarted && exception is EntityNotFoundException)
        {
            _logger.LogError(exception, ErrorMessageTemplate, CheckResponseMessage);

            var problem = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5",
                Title = "Not Found",
                Status = StatusCodes.Status404NotFound,
                Extensions = { { TraceIdKey, Activity.Current?.Id } }
            };

            var result = new NotFoundObjectResult(problem)
            {
                ContentTypes = { ProblemContent }
            };

            await result.ExecuteResultAsync(new ActionContext
            {
                HttpContext = httpContext
            });
        }
        catch (OneBeyondException exception) when (!httpContext.Response.HasStarted)
        {
            _logger.LogError(exception, ErrorMessageTemplate, CheckResponseMessage);

            var problem = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Extensions = { { TraceIdKey, Activity.Current?.Id } }
            };

            var result = new BadRequestObjectResult(problem)
            {
                ContentTypes = { ProblemContent }
            };

            await result.ExecuteResultAsync(new ActionContext
            {
                HttpContext = httpContext
            });
        }
    }
}
