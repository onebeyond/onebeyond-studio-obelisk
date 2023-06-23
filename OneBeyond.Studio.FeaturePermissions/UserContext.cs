using EnsureThat;

namespace OneBeyond.Studio.FeaturePermissions;

[Serializable]
public record UserContext
{
    public UserContext(
            string userAuthId,
            Guid userId,
            string userTypeId,
            string? userRoleId,
            IEnumerable<string>? featurePermissions = null)
    {
        EnsureArg.IsNotNullOrWhiteSpace(userAuthId, nameof(userAuthId));
        EnsureArg.IsNotDefault(userId, nameof(userId));
        EnsureArg.IsNotNullOrWhiteSpace(userTypeId, nameof(userTypeId));

        UserAuthId = userAuthId;
        UserId = userId;
        UserTypeId = userTypeId;
        UserRoleId = userRoleId;
        FeaturePermissions = featurePermissions ?? Array.Empty<string>();
    }

    public string UserAuthId { get; }
    public Guid UserId { get; }
    public string UserTypeId { get; }
    public string? UserRoleId { get; }
    public IEnumerable<string> FeaturePermissions { get; }
}
