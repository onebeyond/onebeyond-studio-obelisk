namespace OneBeyond.Studio.FeaturePermissions.AmbientContexts;
public sealed record AmbientContext : Application.SharedKernel.AmbientContexts.AmbientContext
{
    public AmbientContext(FeaturePermissionUserContext? userContext = null)
    {
        UserContext = userContext;
    }

    public FeaturePermissionUserContext? UserContext { get; }
}
