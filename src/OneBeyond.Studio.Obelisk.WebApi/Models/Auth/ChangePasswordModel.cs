namespace OneBeyond.Studio.Obelisk.WebApi.Models.Auth;

public sealed record ChangePasswordModel
{
    public string OldPassword { get; init; } = default!;
    public string NewPassword { get; init; } = default!;

}
