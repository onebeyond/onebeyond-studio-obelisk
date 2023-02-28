namespace OneBeyond.Studio.Obelisk.Domain.Features.Users.Commands;

public sealed record ForgotPasswordDto
{
    public string Email { get; init; } = default!;
}
