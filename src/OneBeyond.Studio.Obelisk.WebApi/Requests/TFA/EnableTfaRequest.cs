namespace OneBeyond.Studio.Obelisk.WebApi.Requests.TFA;

public sealed record EnableTfaRequest
{
    public string Code { get; init; } = default!;
}
