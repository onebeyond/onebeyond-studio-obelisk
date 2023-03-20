namespace OneBeyond.Studio.Obelisk.WebApi.Requests;

public abstract record BaseListRequest
{
    public int? Limit { get; init; }
    public int Page { get; init; }
    public string? OrderBy { get; init; }
    public string? ParentId { get; init; }
    public string? Ascending { get; init; }
    public string? ByColumn { get; init; } //Added by vue-tables-2 to differentiate global search / column filtering modes
}
