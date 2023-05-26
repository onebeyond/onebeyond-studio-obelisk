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
using Microsoft.Extensions.Configuration;
using MoreLinq;
using OneBeyond.Studio.Application.SharedKernel.DataAccessPolicies;
using OneBeyond.Studio.Crosscuts.Strings;
using OneBeyond.Studio.DataAccess.EFCore.Projections;
using OneBeyond.Studio.Domain.SharedKernel.Entities;
using OneBeyond.Studio.Obelisk.Application.Repositories;

namespace OneBeyond.Studio.Obelisk.Infrastructure.Data;

//TODO WE NEED AN ATTR TO EXCLUDE A PROP FROM BULK IMPORT

internal sealed record BulkInsertInfoDto
{
    /// <summary>
    /// DataType - eg System.Int32, System.DateTime, System.Boolean - Not required for string types
    /// </summary>
    public string DataType { get; init; } = default!;

    public bool IsNullable { get; init; }

    /// <summary>
    /// Property name on the entity class if it differs from the Key(sql column name)
    /// </summary>
    public string PropertyName { get; init; } = default!;
}

public abstract class RWBulkRepository<TAggregateRoot, TAggregateRootId> :
    RWRepository<TAggregateRoot, TAggregateRootId>,
    IRWBulkRepository<TAggregateRoot, TAggregateRootId>
    where TAggregateRoot : AggregateRoot<TAggregateRootId>
    where TAggregateRootId : notnull
{
    private readonly Dictionary<string, BulkInsertInfoDto> _bulkInsertInfo;

    /// <summary>
    /// Destination table in database
    /// </summary>
    protected abstract string TableName { get; }

    protected RWBulkRepository(
        DomainContext dbContext,
        IRWDataAccessPolicyProvider<TAggregateRoot> rwDataAccessPolicyProvider,
        IEntityTypeProjections<TAggregateRoot> entityTypeProjections)
        : base(dbContext, rwDataAccessPolicyProvider, entityTypeProjections)
    {
        _bulkInsertInfo = GetProperties(typeof(TAggregateRoot));
    }

    private static Dictionary<string, BulkInsertInfoDto> GetProperties(Type type)
    {
        Dictionary<string, BulkInsertInfoDto> testBulkInsertInfo = new();

        PopulateProperties(type, testBulkInsertInfo);

        return testBulkInsertInfo;
    }

    private static void PopulateProperties(Type? type, Dictionary<string, BulkInsertInfoDto> testBulkInsertInfo) { 
        if (type is null)
        {
            return;
        }

        type
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(prop => prop.CanWrite)
            .ForEach(prop => {

                var isNullable = prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>);
                var dataType = isNullable ? prop.PropertyType.GetGenericArguments()[0].FullName! : prop.PropertyType.FullName!;

                testBulkInsertInfo.Add(prop.Name, 
                    new BulkInsertInfoDto
                    {
                        DataType = dataType,
                        IsNullable = isNullable
                    });
            });

        PopulateProperties(type.BaseType, testBulkInsertInfo);

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

        _bulkInsertInfo.ForEach((column) =>
        {
            var col = dataTable.Columns.Add(column.Key);

            if (!column.Value.DataType.IsNullOrWhiteSpace())
            {
                col.DataType = Type.GetType(column.Value.DataType);
            }
            col.AllowDBNull = column.Value.IsNullable;
        });

        return dataTable;
    }

    private DataTable AddDataTableRecords(IEnumerable<TAggregateRoot> entities, DataTable dataTable)
    {
        var entityType = entities.First().GetType();
        entities.ForEach((entity) =>
        {
            var row = dataTable.NewRow();
            _bulkInsertInfo.ForEach((column) =>
            {
                var propertyName = string.IsNullOrEmpty(column.Value.PropertyName) ? column.Key : column.Value.PropertyName;
                //Possibly too slow for nested for loops?
                row[column.Key] = entityType.GetProperty(propertyName)!.GetValue(entity) ?? DBNull.Value;
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
        sqlBulkCopy.DestinationTableName = TableName;

        _bulkInsertInfo.ForEach((column) =>
        {
            sqlBulkCopy.ColumnMappings.Add(column.Key, column.Key);
        });

        await sqlBulkCopy.WriteToServerAsync(dataTable, cancellationToken);

    }
}

