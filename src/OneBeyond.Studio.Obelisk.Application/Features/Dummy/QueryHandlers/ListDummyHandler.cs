using System;
using System.Linq.Expressions;
using OneBeyond.Studio.Application.SharedKernel.QueryHandlers;
using OneBeyond.Studio.Application.SharedKernel.Repositories;
using OneBeyond.Studio.Obelisk.Application.Features.Users.Dto;
using OneBeyond.Studio.Obelisk.Domain.Features.Dummies.Entities;

namespace OneBeyond.Studio.Obelisk.Application.Features.Dummies.QueryHandlers;

internal sealed class ListDummyHandler : ListHandler<DummyDto, Dummy, Guid>
{
    public ListDummyHandler(IRORepository<Dummy, Guid> repository)
        : base(repository)
    {
    }

    protected override Expression<Func<DummyDto, bool>> GetSearchExpression(string searchText)
    {
        searchText = searchText.ToLower();
        return user => !string.IsNullOrEmpty(user.StringValue) && user.StringValue.ToLower().Contains(searchText);
    }
}
