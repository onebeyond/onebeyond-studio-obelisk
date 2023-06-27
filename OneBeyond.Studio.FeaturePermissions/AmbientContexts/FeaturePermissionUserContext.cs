using EnsureThat;
using OneBeyond.Studio.Application;

namespace OneBeyond.Studio.FeaturePermissions.AmbientContexts;

[Serializable]
public record FeaturePermissionUserContext : UserContextBase
{
    public FeaturePermissionUserContext(
            string userAuthId,            
            string userTypeId,
            string? userRoleId,
            IEnumerable<string>? featurePermissions = null)
        : base(userAuthId, userTypeId, userRoleId)
    {
        EnsureArg.IsNotNullOrWhiteSpace(userAuthId, nameof(userAuthId));        
        EnsureArg.IsNotNullOrWhiteSpace(userTypeId, nameof(userTypeId));
        
        FeaturePermissions = featurePermissions ?? Array.Empty<string>();
    }        
    
    public IEnumerable<string> FeaturePermissions { get; }
}
