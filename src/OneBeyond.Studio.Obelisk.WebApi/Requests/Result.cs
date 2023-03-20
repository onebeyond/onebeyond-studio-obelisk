namespace OneBeyond.Studio.Obelisk.WebApi.Requests;

public sealed class Result
{
    public static Result Error(string error)
        => new Result(error);

    private Result(string? error = default)
        => this.error = error;

    public string? error { get; }
}
