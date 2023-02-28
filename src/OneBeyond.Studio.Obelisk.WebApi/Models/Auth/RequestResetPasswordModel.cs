namespace OneBeyond.Studio.Obelisk.WebApi.Models.Auth;

public sealed record RequestResetPasswordModel
{
    public string loginId { get; init; } = default!;
}
