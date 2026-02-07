namespace OneBeyond.Studio.Obelisk.Application.Features.Users.Dto;

public sealed record UpdateUserDto
{
    public required string Email { get; init; }
    public required string UserName { get; init; }
    public string? RoleId { get; init; }
    public bool IsActive { get; init;  }
}
