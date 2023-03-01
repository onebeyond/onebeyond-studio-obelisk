namespace OneBeyond.Studio.Obelisk.WebApi.Requests;

public sealed record ListRequest : BaseListRequest
{
    public string? Search { get; init; }
}
