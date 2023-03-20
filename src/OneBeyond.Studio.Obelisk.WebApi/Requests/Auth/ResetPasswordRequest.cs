namespace OneBeyond.Studio.Obelisk.WebApi.Requests.Auth;

public sealed record ResetPasswordRequest
{
    public string UserName { get; init; } = default!;

    public string Password { get; init; } = default!;

    public string Code { get; init; } = default!;
}
