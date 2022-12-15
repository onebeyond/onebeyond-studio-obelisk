using EnsureThat;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain;

public struct SignInProviderLogin
{
    public SignInProviderLogin(
        string signInProviderId,
        string providerLoginId,
        string signInProviderDisplayName)
    {
        EnsureArg.IsNotNullOrWhiteSpace(signInProviderId, nameof(signInProviderId));
        EnsureArg.IsNotNullOrWhiteSpace(providerLoginId, nameof(providerLoginId));
        EnsureArg.IsNotNullOrWhiteSpace(signInProviderDisplayName, nameof(signInProviderDisplayName));

        SignInProviderId = signInProviderId;
        ProviderLoginId = providerLoginId;
        SignInProviderDisplayName = signInProviderDisplayName;
    }

    public string SignInProviderId { get; }
    public string ProviderLoginId { get; }
    public string SignInProviderDisplayName { get; }
}
