using System;
using System.Linq.Expressions;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Entities;

namespace OneBeyond.Studio.Obelisk.Application.Features.Users.Predicates;

public static class UserPredicates
{
    public static Expression<Func<UserBase, bool>> ByLoginId(string loginId)
        => ByLoginId<UserBase>(loginId);

    public static Expression<Func<TUser, bool>> ByLoginId<TUser>(string loginId)
        where TUser : UserBase
        => (user) => user.LoginId == loginId;
}
