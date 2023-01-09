namespace OneBeyond.Studio.Obelisk.Application.Services.AmbientContexts;

public sealed record AmbientContext : Studio.Application.SharedKernel.AmbientContexts.AmbientContext
{
    public AmbientContext(UserContext? userContext)
    {
        UserContext = userContext;
    }

    public UserContext? UserContext { get; }
}
