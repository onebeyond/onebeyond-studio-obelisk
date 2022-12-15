using System;

namespace OneBeyond.Studio.Obelisk.Application.Features.Users.Dto;

public sealed record WhoAmIDto
{
    public Guid UserId { get; init; }
    public string UserName { get; init; } = default!;
    public string UserTypeId { get; init; } = default!;
    public string? UserRoleId { get; init; }
}
