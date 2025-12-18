using System;

namespace OneBeyond.Studio.Obelisk.Application.Features.Users.Dto;

public sealed record GetUserDto
{
    public Guid Id { get; init; }
    public string LoginId { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string UserName { get; init; } = default!;
    public string TypeId { get; init; } = default!;
    public string? RoleId { get; init; }
    public bool IsActive { get; init; }
    public bool IsLockedOut { get; init; }
}
