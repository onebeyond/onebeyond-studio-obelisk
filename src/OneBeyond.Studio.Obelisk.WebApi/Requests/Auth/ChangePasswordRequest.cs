namespace OneBeyond.Studio.Obelisk.WebApi.Requests.Auth;

public sealed record ChangePasswordRequest
{
    public string OldPassword { get; init; } = default!;
    public string NewPassword { get; init; } = default!;

}
