using System.Linq;
using EnsureThat;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace OneBeyond.Studio.Obelisk.Infrastructure.Data.BulkUpdate;

public sealed class PropertyMapping
{
    public PropertyMapping(
        string propertyName,
        IProperty dbProperty)
    {
        EnsureArg.IsNotNullOrWhiteSpace(propertyName, nameof(propertyName));
        EnsureArg.IsNotNull(dbProperty, nameof(dbProperty));

        var mappedDbColumn = dbProperty.GetTableColumnMappings().First()!;

        PropertyName = propertyName;
        DataType = mappedDbColumn.Column.ProviderClrType.FullName!;
        ColumnName = mappedDbColumn.Column.Name;
        IsNullable = mappedDbColumn.Column.IsNullable;
        ValueConverter = dbProperty.GetValueConverter();

        IsExcluded = false;
    }

    /// <summary>
    /// Property name on the entity class 
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// Corresponding database column name
    /// </summary>
    public string ColumnName { get; } 

    /// <summary>
    /// Property DataType - eg System.Int32, System.DateTime, System.Boolean - Not required for string types
    /// </summary>
    public string DataType { get; } 

    public bool IsNullable { get; }

    /// <summary>
    /// Value converter for complex properties, like SmartEnums
    /// </summary>
    public ValueConverter? ValueConverter { get; }

    /// <summary>
    /// Indicates that the property must be excluded from the bulk update
    /// </summary>
    public bool IsExcluded { get; private set; }

    public void Exclude()
        => IsExcluded = true;
}
