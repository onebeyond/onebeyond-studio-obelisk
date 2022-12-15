namespace OneBeyond.Studio.Obelisk.WebApi.Models;

public abstract record BaseQueryParameters
{
    public int? Limit { get; init; }
    public int Page { get; init; }
    public string? OrderBy { get; init; }
    public string? ParentId { get; init; }
    public string? Ascending { get; init; }
    public string? ByColumn { get; init; } //Added by vue-tables-2 to differentiate global search / column filtering modes
}
