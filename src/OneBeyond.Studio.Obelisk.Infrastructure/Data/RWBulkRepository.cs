using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using MoreLinq;
using OneBeyond.Studio.Application.SharedKernel.DataAccessPolicies;
using OneBeyond.Studio.Crosscuts.Strings;
using OneBeyond.Studio.DataAccess.EFCore.Projections;
using OneBeyond.Studio.Domain.SharedKernel.Entities;
using OneBeyond.Studio.Obelisk.Application.Repositories;
using OneBeyond.Studio.Obelisk.Domain.Attributes;

namespace OneBeyond.Studio.Obelisk.Infrastructure.Data;

public class RWBulkRepository<TAggregateRoot, TAggregateRootId> : RWRepository<TAggregateRoot, TAggregateRootId>, IRWBulkRepository<TAggregateRoot, TAggregateRootId>
    where TAggregateRoot : AggregateRoot<TAggregateRootId>
    where TAggregateRootId : notnull
{ 
    private sealed class EntityMapping
    {
        public string TableName { get; init; } = default!;

        public IList<PropertyMapping> PropertyMappings { get; init; } = default!;
    } 

    private sealed record PropertyMapping
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
    }

    private readonly EntityMapping _typeMapping;

    public RWBulkRepository(
        DomainContext dbContext,
        IRWDataAccessPolicyProvider<TAggregateRoot> rwDataAccessPolicyProvider,
        IEntityTypeProjections<TAggregateRoot> entityTypeProjections)
        : base(dbContext, rwDataAccessPolicyProvider, entityTypeProjections)
    {
        _typeMapping = GetTypeMapping(DbContext, typeof(TAggregateRoot));
    }

    private static EntityMapping GetTypeMapping(DomainContext context, Type type)
        => new EntityMapping
        {
            TableName = context.Model.FindEntityType(type)!.GetTableName()!,
            PropertyMappings = GetTypePropertyMappings(context, typeof(TAggregateRoot))
        };

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
            if (prop.IsDefined(typeof(BulkUpdateExcludeAttribute)))
            {
                continue;
            }

            var mappedDbProperty = dbProperties.FirstOrDefault(p => p.Name == prop.Name);

            if (mappedDbProperty is { }) // if the property is mapped into a db table column
            {
                var mappedDbColumn = mappedDbProperty.GetTableColumnMappings().First()!;

                mappingInfo.Add(
                    new PropertyMapping
                    {
                        PropertyName = parentPropertyName.IsNullOrWhiteSpace() ? prop.Name : $"{parentPropertyName}.{prop.Name}",
                        DataType = mappedDbColumn.Column.ProviderClrType.FullName!, //dataType,
                        ColumnName = mappedDbColumn.Column.Name,
                        IsNullable = mappedDbColumn.Column.IsNullable
                    });
            }
            else
            {
                PopulateProperties(context, prop.PropertyType, null, prop.Name, mappingInfo);
            }
        }

        PopulateProperties(context, type.BaseType, dbProperties, parentPropertyName, mappingInfo);

    }

    public async Task BulkInsertAsync(IEnumerable<TAggregateRoot> entitiesToInsert, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(entitiesToInsert, nameof(entitiesToInsert));
        EnsureArg.IsGt(entitiesToInsert.Count(), 0, nameof(entitiesToInsert));

        var dataTable = CreateBulkInsertDataTable();

        dataTable = AddDataTableRecords(entitiesToInsert, dataTable);

        await PerformBulkInsertAsync(dataTable, cancellationToken);
    }

    private DataTable CreateBulkInsertDataTable()
    {
        var dataTable = new DataTable();

        _typeMapping.PropertyMappings.ForEach((column) =>
        {
            var col = dataTable.Columns.Add(column.ColumnName);

            if (!column.DataType.IsNullOrWhiteSpace())
            {
                col.DataType = Type.GetType(column.DataType);
            }
            col.AllowDBNull = column.IsNullable;
        });

        return dataTable;
    }

    private DataTable AddDataTableRecords(IEnumerable<TAggregateRoot> entities, DataTable dataTable)
    {
        var entityType = entities.First().GetType();

        entities.ForEach((entity) =>
        {
            var row = dataTable.NewRow();
            _typeMapping.PropertyMappings.ForEach((column) =>
            {
                row[column.ColumnName] = GetPropertyValue(entityType, entity, column.PropertyName);
            });

            dataTable.Rows.Add(row);
        });

        return dataTable;
    }

    private async Task PerformBulkInsertAsync(
        DataTable dataTable,
        CancellationToken cancellationToken)
    {
        using var sqlBulkCopy = new SqlBulkCopy(DbContext.Database.GetConnectionString());

        sqlBulkCopy.DestinationTableName = _typeMapping.TableName;

        _typeMapping.PropertyMappings.ForEach((column) =>
        {
            sqlBulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
        });

        await sqlBulkCopy.WriteToServerAsync(dataTable, cancellationToken);

    }

    private static object GetPropertyValue(Type entityType, object entity, string propertyName)
    {
        if (entity is null)
        {
            return DBNull.Value;
        }

        var propertyProperties = propertyName.Split('.');

        var property = entityType.GetProperty(propertyProperties[0])!;

        if (propertyProperties.Length > 1)
        {
            var subEntity = property.GetValue(entity);

            return GetPropertyValue(property.PropertyType, subEntity!, string.Join(".", propertyProperties.Skip(1)));
        }

        return property.GetValue(entity) ?? DBNull.Value;
    }
}

