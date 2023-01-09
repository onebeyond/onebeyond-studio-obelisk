using System.Collections.Generic;
using EnsureThat;
using OneBeyond.Studio.Domain.SharedKernel.Entities;
using OneBeyond.Studio.Application.SharedKernel.Entities.Queries;
using OneBeyond.Studio.Obelisk.WebApi.Helpers;
using OneBeyond.Studio.Obelisk.WebApi.Models;

namespace OneBeyond.Studio.Obelisk.WebApi.Extensions;

internal static class QueryParametersExtension
{
    public static List<TAggregateRootListDTO, TAggregateRoot, TAggregateRootId> ToListQuery<TAggregateRootListDTO, TAggregateRoot, TAggregateRootId>(
        this ListQueryParameters queryParameters,
        Dictionary<string, IReadOnlyCollection<string>> query)
        where TAggregateRoot : DomainEntity<TAggregateRootId>
    {
        EnsureArg.IsNotNull(query, nameof(query));

        query = ControllerHelpers.CleanQuery(query);

        return new List<TAggregateRootListDTO, TAggregateRoot, TAggregateRootId>(
            query,
            queryParameters.Search,
            queryParameters.ParentId,
            ControllerHelpers.GetOrderByFields(queryParameters.OrderBy),
            ControllerHelpers.GetSortDirection(queryParameters.Ascending),
            ControllerHelpers.GetPageNumber(queryParameters.Page),
            queryParameters.Limit);

    }
}
