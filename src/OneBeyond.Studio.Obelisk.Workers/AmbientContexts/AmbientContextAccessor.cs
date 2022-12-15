using OneBeyond.Studio.Domain.SharedKernel.AmbientContexts;
using AmbientContext = OneBeyond.Studio.Obelisk.Application.Services.AmbientContexts.AmbientContext;

namespace OneBeyond.Studio.Obelisk.Workers.AmbientContexts;

internal sealed class AmbientContextAccessor : IAmbientContextAccessor<AmbientContext>
{
    public AmbientContextAccessor()
    {
        AmbientContext = new AmbientContext(userContext: default);
    }

    public AmbientContext AmbientContext { get; }
}
