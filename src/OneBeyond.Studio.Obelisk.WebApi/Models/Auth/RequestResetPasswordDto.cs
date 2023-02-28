namespace OneBeyond.Studio.Obelisk.WebApi.Models.Auth;

public sealed record RequestResetPasswordDto
{
    public string loginId { get; init; } = default!;
}
