using System;
using System.Linq.Expressions;
using OneBeyond.Studio.Obelisk.Domain.Features.Dummies.Entities;

namespace OneBeyond.Studio.Obelisk.Application.Features.Dummies.Predicates;

public static class DummyPredicates
{
    public static Expression<Func<Dummy, bool>> ById(Guid id)
        => ById<Dummy>(id);

    public static Expression<Func<TDummy, bool>> ById<TDummy>(Guid id)
        where TDummy : Dummy
        => (dummy) => dummy.Id == id;
}
