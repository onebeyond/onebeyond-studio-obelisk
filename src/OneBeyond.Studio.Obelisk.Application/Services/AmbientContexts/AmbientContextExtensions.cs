using EnsureThat;
using OneBeyond.Studio.Obelisk.Application.Exceptions;

namespace OneBeyond.Studio.Obelisk.Application.Services.AmbientContexts;

public static class AmbientContextExtensions
{
    public static UserContext GetUserContext(this AmbientContext ambientContext)
    {
        EnsureArg.IsNotNull(ambientContext, nameof(ambientContext));

        return ambientContext.UserContext
            ?? throw new ObeliskApplicationException(
                "There is no user context in the current ambient context.");
    }
}
