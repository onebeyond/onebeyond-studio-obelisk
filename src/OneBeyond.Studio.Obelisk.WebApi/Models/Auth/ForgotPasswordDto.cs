namespace OneBeyond.Studio.Obelisk.WebApi.Models.Auth;

public sealed record ForgotPasswordDto
{
    public string Email { get; init; } = default!;
}
