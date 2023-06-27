using System;
using EnsureThat;
using OneBeyond.Studio.Application;

namespace OneBeyond.Studio.Obelisk.Application.Services.AmbientContexts;

//This class is not be sealed for the case when someone needs to extend it
[Serializable]
public record UserContext : UserContextBase
{
    public UserContext(string userAuthId, Guid userId, string userTypeId, string? userRoleId)
        : base(userAuthId, userTypeId, userRoleId)
    {
        EnsureArg.IsNotDefault(userId, nameof(userId));

        UserId = userId;
    }

    public Guid UserId { get; init; }
}
