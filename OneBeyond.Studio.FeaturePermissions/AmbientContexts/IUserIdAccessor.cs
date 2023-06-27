using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneBeyond.Studio.FeaturePermissions.AmbientContexts;
public interface IUserIdAccessor
{
    public Guid? GetLoginId();
}
