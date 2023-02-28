namespace OneBeyond.Studio.Obelisk.WebApi.Models.Auth;

public sealed record ResetPasswordDto
{
    public string UserName { get; init; } = default!;

    public string Password { get; init; } = default!;

    public string Code { get; init; } = default!;
}
