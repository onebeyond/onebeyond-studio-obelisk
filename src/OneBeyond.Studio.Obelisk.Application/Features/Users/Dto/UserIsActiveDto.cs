using System;

namespace OneBeyond.Studio.Obelisk.Application.Features.Users.Dto;

public sealed record UserIsActiveDto
{
    public Guid Id { get; init; }
    public bool IsActive { get; init; }
}
