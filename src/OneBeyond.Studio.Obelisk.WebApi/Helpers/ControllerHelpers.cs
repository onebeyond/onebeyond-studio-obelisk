using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using EnsureThat;
using OneBeyond.Studio.Crosscuts.Strings;
using OneBeyond.Studio.Obelisk.WebApi.Models;

namespace OneBeyond.Studio.Obelisk.WebApi.Helpers;

internal static class ControllerHelpers
{
    public const int DEFAULT_PAGE_NUMBER = 1;
    public const int DEFAULT_PAGE_SIZE = 10;

    public const string SORT_ASCENDING_TRUE = "1";

    internal static string[]? GetOrderByFields(string? orderBy)
        => orderBy?.Split(',', StringSplitOptions.RemoveEmptyEntries);

    internal static int GetPageNumber(int page)
        => page < 1 ? DEFAULT_PAGE_NUMBER : page;

    internal static Dictionary<string, IReadOnlyCollection<string>> CleanQuery(Dictionary<string, IReadOnlyCollection<string>> query)
    {
        EnsureArg.IsNotNull(query, nameof(query));

        var caseInvariantDictionary = new Dictionary<string, IReadOnlyCollection<string>>(query, StringComparer.OrdinalIgnoreCase);

        caseInvariantDictionary.Remove(nameof(BaseQueryParameters.Limit));
        caseInvariantDictionary.Remove(nameof(BaseQueryParameters.Page));
        caseInvariantDictionary.Remove(nameof(BaseQueryParameters.OrderBy));
        caseInvariantDictionary.Remove(nameof(BaseQueryParameters.Ascending));
        caseInvariantDictionary.Remove(nameof(BaseQueryParameters.ParentId));

        var keysToRemove = caseInvariantDictionary
            .Where((item) => !(item.Value?.Count > 0))
            .Select((item) => item.Key)
            .ToList();

        keysToRemove
            .ForEach((key) => caseInvariantDictionary.Remove(key));

        return caseInvariantDictionary;
    }

    internal static ListSortDirection GetSortDirection(string? ascending)
        => ascending.IsNullOrEmpty() || ascending == SORT_ASCENDING_TRUE
            ? ListSortDirection.Ascending
            : ListSortDirection.Descending;
}
