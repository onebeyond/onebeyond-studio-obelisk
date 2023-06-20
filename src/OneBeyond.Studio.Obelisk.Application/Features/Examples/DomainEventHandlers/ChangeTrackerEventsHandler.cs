using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OneBeyond.Studio.Application.SharedKernel.DomainEvents;
using OneBeyond.Studio.Application.SharedKernel.Repositories;
using OneBeyond.Studio.ChangeTracker.Domain.Entities;
using OneBeyond.Studio.Obelisk.Domain.Features.Audit.Entities;

namespace OneBeyond.Studio.Obelisk.Application.Features.Examples.DomainEventHandlers;

internal sealed class ChangeTrackerEventsHandler: IPostSaveDomainEventHandler<ChangeTrackerEvent>
{
    private readonly IRWRepository<AuditRecord, int> _repository;

    public ChangeTrackerEventsHandler(
        IRWRepository<AuditRecord, int> repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(
        ChangeTrackerEvent domainEvent,
        IReadOnlyDictionary<string, object> domainEventAmbientContext,
        CancellationToken cancellationToken)
    {
        //TODO RETRIEVE USER ID FROM AMBIENT CONTEXT

        var auditRecord = new AuditRecord(
            domainEvent.DateRaised,
            "???",
            domainEvent.EntityId,
            domainEvent.EntityType,
            domainEvent.ChangeType,
            JsonConvert.SerializeObject(domainEvent.Changes),
            JsonConvert.SerializeObject(domainEvent.Children));

        await _repository.CreateAsync(auditRecord, cancellationToken);
    }
}
;
