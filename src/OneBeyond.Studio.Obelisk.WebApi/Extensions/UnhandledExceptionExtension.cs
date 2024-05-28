using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OneBeyond.Studio.Crosscuts.Logging;

namespace OneBeyond.Studio.Obelisk.WebApi.Extensions;

internal static class UnhandledExceptionExtension
{
    private static readonly ILogger Logger = LogManager.CreateLogger(nameof(UnhandledExceptionExtension));

    public static void UseUnhandledExceptionLogging(this IApplicationBuilder applicationBuilder)
        => applicationBuilder.Use(LogUnhandledException);

    private static async Task LogUnhandledException(HttpContext httpContext, Func<Task> next)
    {
        try
        {
            await next();
        }
        catch (Exception exception)
        {
            Logger.LogError(exception, "An unhandled exception has occured while processing HTTP request");

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await httpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("An unexpected error has occurred"));

            throw;
        }
    }
}
