namespace OneBeyond.Studio.Obelisk.WebApi.Requests.Auth;

public sealed record RequestResetPasswordRequest
{
    public string LoginId { get; init; } = default!;
}
