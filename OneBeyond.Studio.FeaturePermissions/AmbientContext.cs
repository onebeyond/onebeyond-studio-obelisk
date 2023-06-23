using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneBeyond.Studio.FeaturePermissions;
public sealed record AmbientContext : OneBeyond.Studio.Application.SharedKernel.AmbientContexts.AmbientContext
{
    public AmbientContext(UserContext? userContext = null)
    {
        UserContext = userContext;
    }

    public UserContext? UserContext { get; }
}
