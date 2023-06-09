using System.Collections.Generic;

namespace OneBeyond.Studio.Obelisk.Infrastructure.Data.BulkUpdate;

public sealed record EntityTypeMapping
{
    public string TableName { get; init; } = default!;

    public IList<PropertyMapping> PropertyMappings { get; init; } = default!;

}
