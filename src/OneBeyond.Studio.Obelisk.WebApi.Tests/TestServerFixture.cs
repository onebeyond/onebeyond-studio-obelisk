using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using OneBeyond.Studio.Application.SharedKernel.Entities.Queries;
using OneBeyond.Studio.Application.SharedKernel.Repositories.Exceptions;
using OneBeyond.Studio.Obelisk.Application.Features.Users.Dto;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Entities;
using OneBeyond.Studio.Obelisk.WebApi.Controllers;
using OneBeyond.Studio.Obelisk.WebApi.Extensions;
using OneBeyond.Studio.Obelisk.WebApi.Helpers;
using OneBeyond.Studio.Obelisk.WebApi.Middlewares;

namespace OneBeyond.Studio.Obelisk.WebApi.Tests;

public sealed class TestServerFixture : IAsyncLifetime
{
    public IHost Host { get; private set; } = default!;
    public HttpClient Client { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        Host = await new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .ConfigureServices((context, services) =>
                    {
                        var options = new ClientApplicationOptions { Url = "fake" };
                        services.TryAddSingleton(Options.Create(options));

                        var mediatorMock = new Mock<IMediator>();
                        mediatorMock
                            .Setup(mediator => mediator.Send(It.IsAny<GetById<GetUserDto, UserBase, Guid>>(),It.IsAny<CancellationToken>()))
                            .ThrowsAsync(new EntityNotFoundException<User, Guid>(Guid.NewGuid()));

                        services.TryAddTransient(_ => mediatorMock.Object);
                        services.TryAddTransient<ClientApplicationLinkGenerator, ClientApplicationLinkGenerator>();

                        services.AddAuthentication(options =>
                        {
                            options.DefaultAuthenticateScheme = TestAuthHandler.AuthenticationScheme;
                        })
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.AuthenticationScheme, _ => { });

                        services.AddControllers()
                            .AddApplicationPart(typeof(UsersController).Assembly);

                        services.AddApiVersioning();
                        services.AddHealthChecks()
                            .AddCheck(HealthCheckExtensions.SelfCheck, () => HealthCheckResult.Healthy());
                    })
                    .Configure(app =>
                    {
                        app.UseRouting();
                        app.Use(async (context, next) =>
                        {
                            using var activity = new System.Diagnostics.Activity("HttpRequest");
                            activity.Start();
                            await next();
                        });
                        app.UseMiddleware<ErrorResultGeneratorMiddleware>();
                        app.UseAuthentication();
                        app.UseAuthorization();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();
                            endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
                            {
                                Predicate = registration => registration.Name.Contains(HealthCheckExtensions.SelfCheck)
                            });
                        });
                    });
            })
            .StartAsync();

        Client = Host.GetTestClient();
    }

    public async Task DisposeAsync()
    {
        Client?.Dispose();

        if (Host is not null)
        {
            await Host.StopAsync();
            Host.Dispose();
        }
    }
}
