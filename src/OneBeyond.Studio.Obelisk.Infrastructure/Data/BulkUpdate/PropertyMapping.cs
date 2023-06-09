using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace OneBeyond.Studio.Obelisk.Infrastructure.Data.BulkUpdate;

public sealed record PropertyMapping
{
    /// <summary>
    /// Property name on the entity class if it differs from the Key(sql column name)
    /// </summary>
    public string PropertyName { get; init; } = default!;

    public string ColumnName { get; init; } = default!;

    /// <summary>
    /// DataType - eg System.Int32, System.DateTime, System.Boolean - Not required for string types
    /// </summary>
    public string DataType { get; init; } = default!;

    public bool IsNullable { get; init; }

    public ValueConverter? ValueConverter { get; init; }
}
