namespace OneBeyond.Studio.Obelisk.Application.Features.Users.Dto;

public sealed record CreateUserDto
{
    public string Email { get; private init; } = default!;
    public string UserName { get; private init; } = default!;
    public string? RoleId { get; private init; }
}
