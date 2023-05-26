using System;
using OneBeyond.Studio.Obelisk.Application.Repositories;
using OneBeyond.Studio.Obelisk.Domain.Features.Examples;

namespace OneBeyond.Studio.Obelisk.Application.Features.Examples.Repositories;

public interface ITodoRWBulkRepository : IRWBulkRepository<TodoItem, Guid>
{
}
