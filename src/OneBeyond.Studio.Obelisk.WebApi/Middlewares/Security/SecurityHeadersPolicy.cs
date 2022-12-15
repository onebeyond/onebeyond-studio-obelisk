using System.Collections.Generic;

namespace OneBeyond.Studio.Obelisk.WebApi.Middlewares.Security;

internal sealed class SecurityHeadersPolicy
{
    public IDictionary<string, string> SetHeaders { get; } = new Dictionary<string, string>();

    public ISet<string> RemoveHeaders { get; } = new HashSet<string>();
}
