using System;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Entities;

namespace OneBeyond.Studio.Obelisk.Application.Features.Users.Dto;

public sealed record DummyDto
{
    public Guid? Id { get; set; }
    public string? StringValue { get; private init; } = default!;
    public double? NumericValue { get; private init; } = default!;
    public bool? BoolValue { get; private init; } = default!;
    public DateTime? DateTimeValue { get; private init; } = default!;
    public Guid? FKValueId { get; private init; } = default!;
}
