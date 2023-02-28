using System;
using MediatR;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Entities;

namespace OneBeyond.Studio.Obelisk.Domain.Features.Dummies.Commands;

public sealed record UpdateDummy : IRequest
{
    public UpdateDummy(
        Guid id,
        string? stringValue,
        double? numericValue,
        bool? boolValue,
        DateTime? dateTimeValue,
        Guid? fkValueId)
    {
        Id = id;
        StringValue = stringValue;
        NumericValue = numericValue;
        BoolValue = boolValue;
        DateTimeValue = dateTimeValue;
        FKValueId = fkValueId;
    }

    public Guid Id { get; set; }
    public string? StringValue { get; }
    public double? NumericValue { get; }
    public bool? BoolValue { get; }
    public DateTime? DateTimeValue { get; }
    public Guid? FKValueId { get; }
}
