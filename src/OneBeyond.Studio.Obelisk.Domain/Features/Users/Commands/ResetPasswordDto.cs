namespace OneBeyond.Studio.Obelisk.Domain.Features.Users.Commands;

public sealed record ResetPasswordDto
{
    public string UserName { get; init; } = default!;

    public string Password { get; init; } = default!;

    public string Code { get; init; } = default!;
}
