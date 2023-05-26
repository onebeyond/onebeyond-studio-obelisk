using System;
using System.Collections.Generic;
using OneBeyond.Studio.Application.SharedKernel.DataAccessPolicies;
using OneBeyond.Studio.DataAccess.EFCore.Projections;
using OneBeyond.Studio.Obelisk.Application.Features.Examples.Repositories;
using OneBeyond.Studio.Obelisk.Domain.Features.Examples;

namespace OneBeyond.Studio.Obelisk.Infrastructure.Data.Examples.Repositories;

internal sealed class TodoRWBulkRepository : RWBulkRepository<TodoItem, Guid>, ITodoRWBulkRepository
{
    public TodoRWBulkRepository(
        DomainContext dbContext,
        IRWDataAccessPolicyProvider<TodoItem> rwDataAccessPolicyProvider,
        IEntityTypeProjections<TodoItem> entityTypeProjections)
        : base(dbContext, rwDataAccessPolicyProvider, entityTypeProjections)
    {
    }

    protected override Dictionary<string, BulkInsertInfoDto> BulkInsertInfo => new Dictionary<string, BulkInsertInfoDto>
        {
            {"Id",new BulkInsertInfoDto { DataType= "System.Guid" } },
            {"Title",new BulkInsertInfoDto {} },
            {"Priority",new BulkInsertInfoDto { DataType= "System.Int32" } },
            {"HouseNo",new BulkInsertInfoDto { DataType= "System.Int32" } },
            {"Address_City",new BulkInsertInfoDto {  } },
            {"Zip",new BulkInsertInfoDto { PropertyName = "ZipCode" } },
            {"AssignedToUserId",new BulkInsertInfoDto { DataType= "System.Guid" } },
            {"CompletiedDate",new BulkInsertInfoDto { DataType= "System.DateTime" } }
         };

    protected override string TableName => "TodoItems";
}
