using System;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OneBeyond.Studio.Crosscuts.Exceptions;
using OneBeyond.Studio.Crosscuts.Logging;
using OneBeyond.Studio.Obelisk.WebApi.Models;

namespace OneBeyond.Studio.Obelisk.WebApi.Extensions;

internal static class ErrorResultGeneratorExtension
{
    public static void UseErrorResultGenerator(this IApplicationBuilder applicationBuilder) =>
        applicationBuilder.Use(GenerateErrorResponse);

    private static async Task GenerateErrorResponse(HttpContext httpContext, Func<Task> next)
    {
        try
        {
            await next();
        }
        //NB: This catch can be removed when the new SharedKernel package will be available
        catch (ValidationException exception)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.WriteAsync(exception.Message, default);
        }
        catch (OneBeyondException exception)
        {
            var logger = LogManager.CreateLogger("web Request Handler");
            logger.LogError(
                exception,
                "Error has occured while processing HTTP request.{CheckErrorResponse}",
                httpContext.Response.HasStarted ? string.Empty : " Check error response");
            if (httpContext.Response.HasStarted)
            {
                throw;
            }

            var result = Result.Error(exception.Message);
            var response = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result));
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.Body.WriteAsync(response);
        }
    }
}
