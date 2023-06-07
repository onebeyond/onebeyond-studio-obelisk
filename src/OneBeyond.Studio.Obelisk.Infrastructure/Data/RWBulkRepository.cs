using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Humanizer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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
    private sealed record MappingInfo
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

    private readonly IList<MappingInfo> _mappingInfo;

    /// <summary>
    /// Destination table in database
    /// </summary>
    private readonly string _tableName;

    public RWBulkRepository(
        DomainContext dbContext,
        IRWDataAccessPolicyProvider<TAggregateRoot> rwDataAccessPolicyProvider,
        IEntityTypeProjections<TAggregateRoot> entityTypeProjections)
        : base(dbContext, rwDataAccessPolicyProvider, entityTypeProjections)
    {
        _mappingInfo = GetMappingInfo(typeof(TAggregateRoot));
        _tableName = typeof(TAggregateRoot).GetCustomAttribute<BulkUpdateTableNameAttribute>()?.Name ?? typeof(TAggregateRoot).Name.Pluralize();
    }

    private static IList<MappingInfo> GetMappingInfo(Type type)
    {
        List<MappingInfo> mappingInfo = new();

        PopulateProperties(type, null, mappingInfo);

        return mappingInfo;
    }

    private static void PopulateProperties(
        Type? type, 
        string? parentPropertyName,
        IList<MappingInfo> mappingInfo) 
    { 
        if (type is null)
        {
            return;
        }

        var properties = type
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(prop => prop.CanWrite)
            .ToList();

        foreach (var prop in properties)
        {
            if (prop.IsDefined(typeof(BulkUpdateExcludeAttribute)))
            {
                continue;
            }

            var isPrimitiveType = 
                prop.PropertyType.IsPrimitive 
                || prop.PropertyType == typeof(string) 
                || prop.PropertyType == typeof(DateTime) 
                || prop.PropertyType == typeof(DateTimeOffset)
                || prop.PropertyType == typeof(Guid);

            var isNullable = prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>);

            if (!(isPrimitiveType || isNullable))
            {
                PopulateProperties(prop.PropertyType, prop.Name, mappingInfo);
            }
            else
            {
                var dataType = isNullable ? prop.PropertyType.GetGenericArguments()[0].FullName! : prop.PropertyType.FullName!;

                var propertyName = parentPropertyName.IsNullOrWhiteSpace() ? prop.Name : $"{parentPropertyName}_{prop.Name}";

                var columnName = prop.IsDefined(typeof(BulkUpdateColumnNameAttribute))
                    ? prop.GetCustomAttribute<BulkUpdateColumnNameAttribute>()!.Name
                    : propertyName;

                mappingInfo.Add(
                    new MappingInfo
                    {
                        ColumnName = columnName,
                        PropertyName = propertyName,
                        DataType = dataType,
                        IsNullable = isNullable, 
                    });
            }
        }

        PopulateProperties(type.BaseType, prefix, mappingInfo);

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

        _mappingInfo.ForEach((column) =>
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
            _mappingInfo.ForEach((column) =>
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

        sqlBulkCopy.DestinationTableName = _tableName;

        _mappingInfo.ForEach((column) =>
        {
            sqlBulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
        });

        await sqlBulkCopy.WriteToServerAsync(dataTable, cancellationToken);

    }

    private static object? GetPropertyValue(Type entityType, object entity, string propertyName)
    {
        var propertyProperties = propertyName.Split('_');

        var property = entityType.GetProperty(propertyProperties[0])!;

        if (propertyProperties.Length > 1)
        {
            var subEntity = property.GetValue(entity);

            return GetPropertyValue(property.PropertyType, subEntity!, string.Join("_", propertyProperties.Skip(1)));
        }

        return property.GetValue(entity) ?? DBNull.Value;
    }
}

