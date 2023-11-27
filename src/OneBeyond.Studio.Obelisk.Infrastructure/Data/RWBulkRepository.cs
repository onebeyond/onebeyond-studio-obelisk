using System;
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
using OneBeyond.Studio.Crosscuts.Strings;
using OneBeyond.Studio.DataAccess.EFCore.Projections;
using OneBeyond.Studio.Domain.SharedKernel.Entities;
using OneBeyond.Studio.Obelisk.Application.Repositories;
using OneBeyond.Studio.Obelisk.Infrastructure.Data.BulkUpdate;

namespace OneBeyond.Studio.Obelisk.Infrastructure.Data;

public class RWBulkRepository<TAggregateRoot, TAggregateRootId> : RWRepository<TAggregateRoot, TAggregateRootId>, IRWBulkRepository<TAggregateRoot, TAggregateRootId>
    where TAggregateRoot : AggregateRoot<TAggregateRootId>
    where TAggregateRootId : notnull
{ 
    private readonly EntityTypeMapping _typeMapping;

    public RWBulkRepository(
        DomainContext dbContext,
        IRWDataAccessPolicyProvider<TAggregateRoot> rwDataAccessPolicyProvider,
        IEntityTypeProjections<TAggregateRoot> entityTypeProjections,
        IBulkUpdateConfiguration<TAggregateRoot, TAggregateRootId> bulkUpdateConfiguration)
        : base(dbContext, rwDataAccessPolicyProvider, entityTypeProjections)
    {
        EnsureArg.IsNotNull(bulkUpdateConfiguration, nameof(bulkUpdateConfiguration));

        _typeMapping = bulkUpdateConfiguration.GetTypeMapping(dbContext);
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

        _typeMapping.PropertyMappings
            .Where(x => !x.IsExcluded)
            .ForEach((column) =>
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
            _typeMapping.PropertyMappings
                .Where(x => !x.IsExcluded)
                .ForEach((column) =>
                {
                    var propValue = GetPropertyValue(entityType, entity, column.PropertyName);

                    row[column.ColumnName] = column.ValueConverter is { } 
                        ? propValue.GetType() == typeof(System.DBNull)
                            ? null 
                            : column.ValueConverter.ConvertToProvider(propValue)
                        : propValue;
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

        _typeMapping.PropertyMappings
            .Where(x => !x.IsExcluded)
            .ForEach((column) =>
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

