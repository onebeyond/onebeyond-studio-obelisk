namespace OneBeyond.Studio.Obelisk.Domain.Features.Users.Commands;

public sealed record RequestResetPasswordDto
{
    public string loginId { get; init; } = default!;
}
