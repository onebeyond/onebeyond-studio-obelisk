using System;
using EnsureThat;
using MediatR;

namespace OneBeyond.Studio.Obelisk.Domain.Features.Dummies.Commands;

public sealed record CreateDummy : IRequest<Guid>
{
    public CreateDummy(
        string? stringValue,
        double? numericValue,
        bool? boolValue,
        DateTime? dateTimeValue,
        Guid? fkValueId)
    {
        StringValue = stringValue;
        NumericValue = numericValue;
        BoolValue = boolValue;
        DateTimeValue = dateTimeValue;
        FKValueId = fkValueId;
    }

    public string? StringValue { get; }
    public double? NumericValue { get; }
    public bool? BoolValue { get; }
    public DateTime? DateTimeValue { get; }
    public Guid? FKValueId { get; }
}
