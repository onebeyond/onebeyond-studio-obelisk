using System.Security.Claims;
using System.Security.Principal;
using EnsureThat;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Exceptions;

namespace OneBeyond.Studio.Obelisk.WebApi.SignInProviders;

public sealed class AzureADSignInProvider : ISignInProvider
{
    public string GetProviderLoginId(IIdentity claimsIdentity)
    {
        EnsureArg.IsNotNull(claimsIdentity, nameof(claimsIdentity));

        const string OidClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        return (claimsIdentity as ClaimsIdentity)!.FindFirst(OidClaimType)?.Value
            ?? throw new AuthException("Failed to retrieve provider loginId.");
    }

    public string GetEmail(IIdentity claimsIdentity)
    {
        EnsureArg.IsNotNull(claimsIdentity, nameof(claimsIdentity));

        return (claimsIdentity as ClaimsIdentity)!.FindFirst(ClaimTypes.Email)?.Value
            ?? throw new AuthException("Failed to retrieve e-mail for extrenaly logged in user.");
    }
}
