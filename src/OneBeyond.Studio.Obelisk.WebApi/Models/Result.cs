namespace OneBeyond.Studio.Obelisk.WebApi.Models;

public sealed class Result
{
    public static Result Error(string error)
        => new Result(error);

    private Result(string? error = default)
        => this.error = error;

    public string? error { get; }
}
