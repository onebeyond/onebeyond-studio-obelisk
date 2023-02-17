namespace OneBeyond.Studio.Obelisk.WebApi.Dapr;

public sealed class DaprConfiguration
{
    public string AppId { get; init; } = default!;
    public bool AppSsl { get; init; }
    public int AppPort { get; init; }
    public int DaprHttpPort { get; init; }
    public int DaprGrpcPort { get; init; }
    public string AppProtocol { get; init; } = default!;
    public string LogLevel { get; init; } = default!;
}
