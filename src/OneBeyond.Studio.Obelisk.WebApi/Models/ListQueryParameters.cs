namespace OneBeyond.Studio.Obelisk.WebApi.Models;

public sealed record ListQueryParameters : BaseQueryParameters
{
    public string? Search { get; init; }
}
