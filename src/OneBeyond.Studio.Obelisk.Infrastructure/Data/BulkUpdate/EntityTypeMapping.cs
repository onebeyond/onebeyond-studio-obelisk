using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using OneBeyond.Studio.Obelisk.Infrastructure.Exceptions;

namespace OneBeyond.Studio.Obelisk.Infrastructure.Data.BulkUpdate;

public sealed class EntityTypeMapping
{
    public EntityTypeMapping(
        string typeName,
        string tableName,
        IList<PropertyMapping> propertyMappings)
    {
        EnsureArg.IsNotNullOrWhiteSpace(typeName, nameof(typeName));
        EnsureArg.IsNotNullOrWhiteSpace(tableName, nameof(tableName));
        EnsureArg.IsNotNull(propertyMappings, nameof(propertyMappings));

        TypeName = typeName;
        TableName = tableName;
        PropertyMappings = propertyMappings;

    }

    public string TypeName { get; }

    public string TableName { get; }

    public IList<PropertyMapping> PropertyMappings { get; }

    public PropertyMapping GetPropertyMapping(string propertyName)
    => FindPropertyMapping(propertyName)
        ?? throw new ObeliskInfrastructureException($"Property {propertyName} not found in bulk update mappings for type {TypeName} ");

    public PropertyMapping? FindPropertyMapping(string propertyName)
        => PropertyMappings.FirstOrDefault(mapping => mapping.PropertyName == propertyName);
}
