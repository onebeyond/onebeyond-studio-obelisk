namespace OneBeyond.Studio.Obelisk.WebApi.Requests.Auth;

public sealed record ForgotPasswordRequest
{
    public string Email { get; init; } = default!;
}
