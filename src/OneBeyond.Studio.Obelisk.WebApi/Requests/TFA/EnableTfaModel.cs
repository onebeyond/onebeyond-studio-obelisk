namespace OneBeyond.Studio.Obelisk.WebApi.Requests.TFA;

public sealed record EnableTfaModel
{
    public string Code { get; init; } = default!;
}
