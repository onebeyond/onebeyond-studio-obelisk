using System;

namespace OneBeyond.Studio.Obelisk.Application.Features.Users.Dto;

public sealed record ListUsersDto
{
    public Guid Id { get; private init; }
    public string LoginId { get; private init; } = default!;
    public string Email { get; private init; } = default!;
    public string UserName { get; private init; } = default!;
    public string TypeId { get; private init; } = default!;
    public string? RoleId { get; private init; }
    public bool IsActive { get; private init; }
    public bool IsLockedOut { get; private init; }
}
