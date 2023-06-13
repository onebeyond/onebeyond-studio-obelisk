using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EnsureThat;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using OneBeyond.Studio.Crosscuts.Strings;
using OneBeyond.Studio.Domain.SharedKernel.Entities;

namespace OneBeyond.Studio.Obelisk.Infrastructure.Data.BulkUpdate;

internal class BulkUpdateConfiguration<TAggregateRoot, TAggregateRootId> : IBulkUpdateConfiguration<TAggregateRoot, TAggregateRootId>
    where TAggregateRoot : AggregateRoot<TAggregateRootId>
    where TAggregateRootId : notnull
{
    private sealed class TypePropertyScanner
    {
        private readonly DbContext _context;
        private readonly Type _type;
        private readonly string _typeTableName;

        public TypePropertyScanner(DbContext context, Type type)
        {
            _context = EnsureArg.IsNotNull(context, nameof(context));
            _type = EnsureArg.IsNotNull(type, nameof(type));
            //BulkUpdate supports update only of one table. 
            //We do not support cases when one BulkUpdateRepository<type> bulk updates multiple tables.
            //So, we get the name of the table the type is mapped to and gather information only about this table.
            //You'll see in the recursion that if a type is mapped into another table, this is a navigation property, and we do not include it into the bulk update.
            _typeTableName = context.Model.FindEntityType(type)!.GetTableName()!;
        }

        public IList<PropertyMapping> ScanTypeProperties()
        {
            var mappingInfo = new List<PropertyMapping>();

            PopulateTypeProperties(_type, null, mappingInfo);

            return mappingInfo;
        }

        private void PopulateTypeProperties(
            Type? type,
            string? parentPropertyName,
            IList<PropertyMapping> mappingInfo)
        {
            if (type is null)
            {
                return;
            }

            var typeTableName = _context.Model.FindEntityType(type)?.GetTableName();

            if (typeTableName != _typeTableName)
            {
                return; //we gather properties that are mapped into the parentTypeTableName only. If the type is mapped into another table, we do not include it into the bulk update.
            }

            PopulateMappedTypeProperties(
                type,
                _context.Model.FindEntityType(type)!.GetProperties(),
                parentPropertyName,
                mappingInfo);
        }

        private void PopulateMappedTypeProperties(
            Type? type,
            IEnumerable<IProperty> dbProperties,
            string? parentPropertyName,
            IList<PropertyMapping> mappingInfo)
        {
            if (type is null)
            {
                return;
            }

            var properties = type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(prop => prop.CanWrite && !IsCollection(prop.PropertyType)) //Note, we exclude all collections, as we don't want to update them when doing bulk update
                .ToList();

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
                    PopulateTypeProperties(prop.PropertyType, prop.Name, mappingInfo);
                }
            }

            PopulateMappedTypeProperties(type.BaseType, dbProperties, parentPropertyName, mappingInfo);
        }

        private static bool IsCollection(Type type)
            => typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string);

    }

    public virtual EntityTypeMapping GetTypeMapping(DomainContext context)
        => GetTypeMapping(context, typeof(TAggregateRoot));

    private static EntityTypeMapping GetTypeMapping(DomainContext context, Type type)
        => new EntityTypeMapping(
            type.Name,
            context.Model.FindEntityType(type)!.GetTableName()!,
            new TypePropertyScanner(context, typeof(TAggregateRoot)).ScanTypeProperties());
}
