using System;
using System.Threading.Tasks;
using System.Threading;
using EnsureThat;
using MediatR;
using Microsoft.Extensions.Primitives;
using OneBeyond.Studio.Domain.SharedKernel.Entities;
using OneBeyond.Studio.Obelisk.Domain.Features.Dummies.Commands;
using OneBeyond.Studio.Obelisk.Domain.Features.Dummies.DomainEvents;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Entities;

namespace OneBeyond.Studio.Obelisk.Domain.Features.Dummies.Entities;

public class Dummy : DomainEntity<Guid>, IAggregateRoot
{
    protected Dummy(
        string? stringValue,
        double? numericValue,
        bool? boolValue,
        DateTime? dateTimeValue,
        Guid? fkValue
        )
        : base(Guid.NewGuid())
    {
        StringValue = stringValue;
        NumericValue = numericValue;
        BoolValue = boolValue;
        DateTimeValue = dateTimeValue;
        FKValueId = fkValue;

        RaiseDomainEvent(new DummyCreated(Id));
    }

#nullable disable
    protected Dummy()
    {
    }
#nullable restore

    public string? StringValue { get; private set; } = default!;
    public double? NumericValue { get; private set; } = default!;
    public bool? BoolValue { get; private set; } = default!;
    public DateTime? DateTimeValue { get; private set; } = default!;
    public Guid? FKValueId { get; private set; } = default!;

    public static Dummy Apply(CreateDummy command)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        return new Dummy(
            command.StringValue,
            command.NumericValue,
            command.BoolValue,
            command.DateTimeValue,
            command.FKValueId);
    }

    public void Apply(UpdateDummy command)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        StringValue = command.StringValue;
        NumericValue = command.NumericValue;
        BoolValue = command.BoolValue;
        DateTimeValue = command.DateTimeValue;
        FKValueId = command.FKValueId;
    }
}
