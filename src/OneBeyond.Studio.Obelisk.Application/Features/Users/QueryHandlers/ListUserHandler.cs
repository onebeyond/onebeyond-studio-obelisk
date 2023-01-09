using System;
using System.Linq.Expressions;
using OneBeyond.Studio.Application.SharedKernel.QueryHandlers;
using OneBeyond.Studio.Application.SharedKernel.Repositories;
using OneBeyond.Studio.Obelisk.Application.Features.Users.Dto;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Entities;

namespace OneBeyond.Studio.Obelisk.Application.Features.Users.QueryHandlers;

internal sealed class ListUserHandler : ListHandler<ListUsersDto, UserBase, Guid>
{
    public ListUserHandler(IRORepository<UserBase, Guid> repository)
        : base(repository)
    {
    }

    protected override Expression<Func<ListUsersDto, bool>> GetSearchExpression(string searchText)
    {
        searchText = searchText.ToLower();
        return user => !string.IsNullOrEmpty(user.UserName) && user.UserName.ToLower().Contains(searchText);
    }
}
