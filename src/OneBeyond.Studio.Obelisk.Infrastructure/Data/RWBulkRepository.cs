using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MoreLinq;
using OneBeyond.Studio.Application.SharedKernel.DataAccessPolicies;
using OneBeyond.Studio.DataAccess.EFCore.Projections;
using OneBeyond.Studio.Domain.SharedKernel.Entities;
using OneBeyond.Studio.Obelisk.Application.Repositories;

namespace OneBeyond.Studio.Obelisk.Infrastructure.Data;

public sealed record BulkInsertInfoDto
{
    /// <summary>
    /// DataType - eg System.Int32, System.DateTime, System.Boolean - Not required for string types
    /// </summary>
    public string DataType { get; init; } = default!;

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
    /// <summary>
    /// Info used to build the bulk insert data table. 
    /// Key is the Sql Column name, BulkInsertInfo - DataType - C# data type, PropertyName - Name of C# property - if empty Key is used
    /// </summary>
    protected abstract Dictionary<string, BulkInsertInfoDto> BulkInsertInfo { get; }

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
    }

    public async Task BulkInsertAsync(IEnumerable<TAggregateRoot> entitiesToInsert, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(entitiesToInsert, nameof(entitiesToInsert));
        EnsureArg.IsGt(0, entitiesToInsert.Count(), nameof(entitiesToInsert));

        var dataTable = CreateBulkInsertDataTable();

        dataTable = AddDataTableRecords(entitiesToInsert, dataTable);

        await PerformBulkInsertAsync(dataTable, cancellationToken);
    }

    private DataTable CreateBulkInsertDataTable()
    {
        var dataTable = new DataTable();
        BulkInsertInfo.ForEach((column) =>
        {
            if (!string.IsNullOrEmpty(column.Value.DataType))
            {
                dataTable.Columns.Add(column.Key).DataType = System.Type.GetType(column.Value.DataType);
            }
            else
            {
                dataTable.Columns.Add(column.Key);
            }
        });

        return dataTable;
    }

    private DataTable AddDataTableRecords(IEnumerable<TAggregateRoot> entities, DataTable dataTable)
    {
        var entityType = entities.First().GetType();
        entities.ForEach((entity) =>
        {
            var row = dataTable.NewRow();
            BulkInsertInfo.ForEach((column) =>
            {
                var propertyName = string.IsNullOrEmpty(column.Value.PropertyName) ? column.Key : column.Value.PropertyName;
                //Possibly too slow for nested for loops?
                row[column.Key] = entityType.GetProperty(propertyName)!.GetValue(entity);
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

        BulkInsertInfo.ForEach((column) =>
        {
            sqlBulkCopy.ColumnMappings.Add(column.Key, column.Key);
        });

        await sqlBulkCopy.WriteToServerAsync(dataTable, cancellationToken);

    }
}

