using System.Security.Claims;
using EnsureThat;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain;

public sealed class ExternalLoginInfo
{
    public ExternalLoginInfo(
        ClaimsPrincipal principal,
        string loginProvider,
        string providerKey,
        string displayName)
    {
        EnsureArg.IsNotNull(principal, nameof(principal));
        EnsureArg.IsNotNullOrWhiteSpace(loginProvider, nameof(loginProvider));
        EnsureArg.IsNotNullOrWhiteSpace(providerKey, nameof(providerKey));
        EnsureArg.IsNotNullOrWhiteSpace(displayName, nameof(displayName));

        Principal = principal;
        LoginProvider = loginProvider;
        ProviderKey = providerKey;
        DisplayName = displayName;
    }

    public ClaimsPrincipal Principal { get; }
    public string LoginProvider { get; }
    public string ProviderKey { get; }
    public string DisplayName { get; }
}
