namespace OneBeyond.Studio.Obelisk.Domain.Features.Users.Commands;

public sealed record ChangePasswordDto
{
    public string OldPassword { get; init; } = default!;
    public string NewPassword { get; init; } = default!;

}
