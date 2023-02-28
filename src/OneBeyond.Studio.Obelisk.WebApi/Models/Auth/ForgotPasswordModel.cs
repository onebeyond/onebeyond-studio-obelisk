namespace OneBeyond.Studio.Obelisk.WebApi.Models.Auth;

public sealed record ForgotPasswordModel
{
    public string Email { get; init; } = default!;
}
