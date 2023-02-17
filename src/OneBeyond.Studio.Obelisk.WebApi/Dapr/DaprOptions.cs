namespace OneBeyond.Studio.Obelisk.WebApi.Dapr;

public sealed class DaprOptions
{
    public bool UseSelfHosted { get; init; }
    public DaprConfiguration Configuration { get; init; } = default!;
}
