using System;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using OneBeyond.Studio.Application.SharedKernel.UnitsOfWork;

namespace OneBeyond.Studio.Obelisk.WebApi.Middlewares.UnitOfWork;

internal static class ApplicationBuilderExtensions
{
    public static void UseUnitOfWork(this IApplicationBuilder applicationBuilder)
    {
        EnsureArg.IsNotNull(applicationBuilder, nameof(applicationBuilder));

        // Check if IUnitOfWork is registered.
        using (var scope = applicationBuilder.ApplicationServices.CreateScope())
        using (var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>())
        {
            if (unitOfWork == null)
            {
                throw new InvalidOperationException($"{nameof(IUnitOfWork)} is not registered in DI container.");
            }
        }

        // Register unit of work scope.
        applicationBuilder.UseMiddleware<UnitOfWorkScope>();
    }

    public sealed class UnitOfWorkScope
    {
        private readonly RequestDelegate _next;

        public UnitOfWorkScope(RequestDelegate next)
        {
            EnsureArg.IsNotNull(next, nameof(next));

            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            using (var scope = httpContext.RequestServices.CreateScope())
            using (var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>())
            {
                await _next(httpContext);
                await unitOfWork.CompleteAsync();
            }
        }
    }
}
