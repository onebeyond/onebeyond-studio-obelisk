using System;

namespace OneBeyond.Studio.Obelisk.Application.Features.Users.Dto;

public sealed record UserNameDto
{
    public Guid UserId { get; init; }
    public string UserName { get; init; } = default!;
}
