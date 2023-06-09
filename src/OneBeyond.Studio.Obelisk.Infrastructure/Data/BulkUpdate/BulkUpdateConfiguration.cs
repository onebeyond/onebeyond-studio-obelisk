using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using OneBeyond.Studio.Crosscuts.Strings;
using OneBeyond.Studio.Domain.SharedKernel.Entities;

namespace OneBeyond.Studio.Obelisk.Infrastructure.Data.BulkUpdate;

internal class BulkUpdateConfiguration<TAggregateRoot, TAggregateRootId> : IBulkUpdateConfiguration<TAggregateRoot, TAggregateRootId>
    where TAggregateRoot : AggregateRoot<TAggregateRootId>
    where TAggregateRootId : notnull
{
    public virtual EntityTypeMapping GetTypeMapping(DomainContext context)
        => GetTypeMapping(context, typeof(TAggregateRoot));

    private static EntityTypeMapping GetTypeMapping(DomainContext context, Type type)
        => new EntityTypeMapping(
            type.Name,
            context.Model.FindEntityType(type)!.GetTableName()!,
            GetTypePropertyMappings(context, typeof(TAggregateRoot)));

    private static IList<PropertyMapping> GetTypePropertyMappings(
        DbContext context,
        Type type)
    {
        List<PropertyMapping> mappingInfo = new();

        PopulateProperties(context, type, null, null, mappingInfo);

        return mappingInfo;
    }

    private static void PopulateProperties(
        DbContext context,
        Type? type,
        IEnumerable<IProperty>? dbProperties,
        string? parentPropertyName,
        IList<PropertyMapping> mappingInfo)
    {
        if (type is null)
        {
            return;
        }

        var properties = type
            .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Where(prop => prop.CanWrite)
            .ToList();

        if (dbProperties is null)
        {
            dbProperties = context.Model.FindEntityType(type)!.GetProperties();
        }

        foreach (var prop in properties)
        {
            var mappedDbProperty = dbProperties.FirstOrDefault(p => p.Name == prop.Name);

            if (mappedDbProperty is { }) // if the property is mapped into a db table column
            {
                mappingInfo.Add(
                    new PropertyMapping(
                        parentPropertyName.IsNullOrWhiteSpace() ? prop.Name : $"{parentPropertyName}.{prop.Name}",
                        mappedDbProperty));
            }
            else
            {
                PopulateProperties(context, prop.PropertyType, null, prop.Name, mappingInfo);
            }
        }

        PopulateProperties(context, type.BaseType, dbProperties, parentPropertyName, mappingInfo);

    }

}
