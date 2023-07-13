using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OneBeyond.Studio.Application.SharedKernel.Repositories;
using OneBeyond.Studio.Domain.SharedKernel.Entities;

namespace OneBeyond.Studio.Obelisk.Application.Repositories;

public interface IRWBulkRepository<TAggregateRoot, TAggregateRootId> : IRWRepository<TAggregateRoot, TAggregateRootId>
    where TAggregateRoot : AggregateRoot<TAggregateRootId>
    where TAggregateRootId : notnull
{
    Task BulkInsertAsync(IEnumerable<TAggregateRoot> entitiesToInsert, CancellationToken cancellationToken);
}
