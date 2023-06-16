namespace OneBeyond.Studio.Obelisk.WebApi.Requests.Auth;

public sealed record ResetPasswordRequest
{
    public string UserId { get; init; } = default!;

    public string Token { get; init; } = default!;

    public string Password { get; init; } = default!;
}
