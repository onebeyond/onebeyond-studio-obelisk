using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneBeyond.Studio.Application.AmbientContexts;
internal interface IUserIdSetter
{
    public void SetUserid(Guid aspNetUserId);
}
