namespace OneBeyond.Studio.FeaturePermissions.AmbientContexts;
public sealed record AmbientContext : Application.SharedKernel.AmbientContexts.AmbientContext
{
    public AmbientContext(UserContext? userContext = null)
    {
        UserContext = userContext;
    }

    public UserContext? UserContext { get; }
}
