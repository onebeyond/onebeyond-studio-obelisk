using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using OneBeyond.Studio.Application.SharedKernel.Entities.Queries;
using OneBeyond.Studio.Application.SharedKernel.Repositories.Exceptions;
using OneBeyond.Studio.Core.Mediator;
using OneBeyond.Studio.Obelisk.Application.Features.Users.Dto;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Entities;
using OneBeyond.Studio.Obelisk.WebApi.Controllers;
using OneBeyond.Studio.Obelisk.WebApi.Helpers;
using OneBeyond.Studio.Obelisk.WebApi.Middlewares;

namespace OneBeyond.Studio.Obelisk.WebApi.Tests;

public sealed class TestServerFixture : IAsyncLifetime
{
    public WebApplication App { get; private set; } = default!;
    public HttpClient Client { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();

        var options = new ClientApplicationOptions { Url = "fake" };
        builder.Services.TryAddSingleton(Options.Create(options));

        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<GetById<GetUserDto, UserBase, Guid>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new EntityNotFoundException<User, Guid>(Guid.NewGuid()));

        builder.Services.TryAddTransient(_ => mediatorMock.Object);
        builder.Services.TryAddTransient<ClientApplicationLinkGenerator, ClientApplicationLinkGenerator>();

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = TestAuthHandler.AuthenticationScheme;
        })
        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.AuthenticationScheme, _ => { });

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(UsersController).Assembly);

        builder.Services.AddApiVersioning();
        builder.AddDefaultHealthChecks();

        var app = builder.Build();

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
        app.MapControllers();
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = r => r.Tags.Contains("live")
        });

        await app.StartAsync();
        App = app;
        Client = app.GetTestClient();
    }

    public async Task DisposeAsync()
    {
        Client?.Dispose();

        if (App is not null)
        {
            await App.StopAsync();
            await App.DisposeAsync();
        }
    }
}
