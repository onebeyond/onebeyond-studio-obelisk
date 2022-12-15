using System.Security.Principal;

namespace OneBeyond.Studio.Obelisk.WebApi.SignInProviders;

//It was decided to leave this interface in WebAPI project, and later when other external Sign In providers added - we'll decide where to move it.
public interface ISignInProvider
{
    public string GetProviderLoginId(IIdentity claimsIdentity);

    public string GetEmail(IIdentity claimsIdentity);
}
