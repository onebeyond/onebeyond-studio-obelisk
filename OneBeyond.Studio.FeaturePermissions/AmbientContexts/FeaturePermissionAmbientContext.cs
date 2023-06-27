namespace OneBeyond.Studio.FeaturePermissions.AmbientContexts;
public record FeaturePermissionAmbientContext : Application.SharedKernel.AmbientContexts.AmbientContext
{
    public FeaturePermissionAmbientContext(FeaturePermissionUserContext? userContext = null)
    {
        UserContext = userContext;
    }

    public FeaturePermissionUserContext? UserContext { get; }
}
