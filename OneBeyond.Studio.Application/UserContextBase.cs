using EnsureThat;

namespace OneBeyond.Studio.Application;
public abstract record UserContextBase
{
    public UserContextBase(string userAuthId, string userTypeId, string? userRoleId)
    {
        EnsureArg.IsNotNullOrWhiteSpace(userAuthId, nameof(userAuthId));        
        EnsureArg.IsNotNullOrWhiteSpace(userTypeId, nameof(userTypeId));

        UserAuthId = userAuthId;        
        UserTypeId = userTypeId;
        UserRoleId = userRoleId;
    }

    public string UserAuthId { get; }    
    public string UserTypeId { get; }
    public string? UserRoleId { get; }
}
