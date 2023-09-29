using System;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.Extensions.Logging;
using OneBeyond.Studio.Application.SharedKernel.IntegrationEvents;
using OneBeyond.Studio.Crosscuts.Logging;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.DomainEvents;

namespace OneBeyond.Studio.Obelisk.Application.Features.Users.IntegrationEventHandlers;

internal class UserCreatedIntegrationEventHandler : IIntegrationEventHandler<UserCreated>
{
    private static readonly ILogger Logger = LogManager.CreateLogger<UserCreatedIntegrationEventHandler>();

    public Task HandleAsync(UserCreated integrationEvent, CancellationToken cancellationToken = default)
    {
        EnsureArg.IsNotNull(integrationEvent, nameof(integrationEvent));
        Logger.LogInformation("Integration event received. User created: {UserId}", integrationEvent.UserId);
        return Task.CompletedTask;
    }
}
