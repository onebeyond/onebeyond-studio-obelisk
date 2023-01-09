using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OneBeyond.Studio.Application.SharedKernel.DomainEvents;
using OneBeyond.Studio.Crosscuts.Logging;
using OneBeyond.Studio.Crosscuts.MessageQueues;

namespace OneBeyond.Studio.Obelisk.WebApi.HostedServices;

// It is worth bearing in mind that this background service is scaled along with
// the host. In general, there will be no any issue with this, but the database
// will be polled from multiple processes. There is a number of mitigations for that:
// 1. Deployment into a separate host which is scaled independently
// 2. Use of a distributed lock around this code (https://github.com/madelson/DistributedLock).
//    This would lead to polling of some other resource used for distributed lock.
//    So you need to choose what is preferable.
// When taking a decision on the above, never stem out of a desire to have domain events coming
// in order. They will be published on some messaging system anyway which never guaranties
// order of domain event consumption.
internal sealed class DomainEventRelayJob : BackgroundService
{
    private static readonly ILogger Logger = LogManager.CreateLogger<DomainEventRelayJob>();

    private readonly IRaisedDomainEventReceiver _raisedDomainEventReceiver;
    private readonly IMessageQueue<RaisedDomainEvent> _raisedDomainEventMessageQueue;

    public DomainEventRelayJob(
        IRaisedDomainEventReceiver raisedDomainEventReceiver,
        IMessageQueue<RaisedDomainEvent> raisedDomainEventMessageQueue)
    {
        EnsureArg.IsNotNull(raisedDomainEventReceiver, nameof(raisedDomainEventReceiver));
        EnsureArg.IsNotNull(raisedDomainEventMessageQueue, nameof(raisedDomainEventMessageQueue));

        _raisedDomainEventReceiver = raisedDomainEventReceiver;
        _raisedDomainEventMessageQueue = raisedDomainEventMessageQueue;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return _raisedDomainEventReceiver.RunAsync(
            (raisedDomainEvent, cancellationToken) =>
            {
                Logger.LogInformation(
                    "Domain event {@RaisedDomainEvent} arrived from receiver. Publishing it on the queue...",
                    raisedDomainEvent);

                return _raisedDomainEventMessageQueue.PublishAsync(raisedDomainEvent, cancellationToken);
            },
            stoppingToken);
    }
}
