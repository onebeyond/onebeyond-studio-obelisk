using System;
using MediatR;

namespace OneBeyond.Studio.Obelisk.Domain.Features.Dummies.Commands;

public sealed record DeleteDummy : IRequest
{
    public DeleteDummy(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }
}
