using System;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using OneBeyond.Studio.Application.SharedKernel.DomainEvents;
using OneBeyond.Studio.Crosscuts.Logging;

namespace OneBeyond.Studio.Obelisk.Workers.Functions;

internal sealed class DomainEventProcessor
{
    // These are intentionally single-underscored to allow deployment to
    // both windows and linux.
    private const string QueueConnection = "DomainEvents_Queue_ConnectionString";
    private const string QueueName = "DomainEvents_Queue_QueueName";

    private static readonly ILogger Logger = LogManager.CreateLogger<DomainEventProcessor>();

    private readonly IPostSaveDomainEventDispatcher _postSaveDomainEventDispatcher;

    public DomainEventProcessor(IPostSaveDomainEventDispatcher postSaveDomainEventDispatcher)
    {
        EnsureArg.IsNotNull(postSaveDomainEventDispatcher, nameof(postSaveDomainEventDispatcher));
        _postSaveDomainEventDispatcher = postSaveDomainEventDispatcher;
    }

    [Function(nameof(DomainEventProcessor))]
    public async Task RunAsync(
        [QueueTrigger(
            $"%{QueueName}%",
            Connection = QueueConnection)]
        RaisedDomainEvent raisedDomainEvent,
        CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(raisedDomainEvent, nameof(raisedDomainEvent));

        var domainEvent = raisedDomainEvent.GetValue();
        var domainEventType = domainEvent.GetType().FullName;
        var domainEventAmbientContext = raisedDomainEvent.GetAmbientContext();

        try
        {
            Logger.LogInformation(
                "Processing domain event of the {DomainEventType} type for entity {EntityId}",
                domainEventType,
                raisedDomainEvent.EntityId);

            await _postSaveDomainEventDispatcher.DispatchAsync(
                    domainEvent,
                    domainEventAmbientContext,
                    cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            Logger.LogError(
                exception,
                "Unable to process domain event {@DomainEvent} of the {DomainEventType} type for entity {EntityId}",
                domainEvent,
                domainEventType,
                raisedDomainEvent.EntityId);
            throw;
        }
    }
}
