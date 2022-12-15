using System;
using EnsureThat;

namespace OneBeyond.Studio.Obelisk.Application.Services.AmbientContexts;

//This class is not be sealed for the case when someone needs to extend it
[Serializable]
public record UserContext
{
    public UserContext(string userAuthId, Guid userId, string userTypeId, string? userRoleId)
    {
        EnsureArg.IsNotNullOrWhiteSpace(userAuthId, nameof(userAuthId));
        EnsureArg.IsNotDefault(userId, nameof(userId));
        EnsureArg.IsNotNullOrWhiteSpace(userTypeId, nameof(userTypeId));

        UserAuthId = userAuthId;
        UserId = userId;
        UserTypeId = userTypeId;
        UserRoleId = userRoleId;
    }

    public string UserAuthId { get; }
    public Guid UserId { get; }
    public string UserTypeId { get; }
    public string? UserRoleId { get; }
}
