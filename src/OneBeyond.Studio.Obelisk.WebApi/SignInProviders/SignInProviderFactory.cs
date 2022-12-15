using System;
using EnsureThat;

namespace OneBeyond.Studio.Obelisk.WebApi.SignInProviders;

internal static class SignInProviderFactory
{
    public static ISignInProvider GetSignInProvider(string providerId)
    {
        EnsureArg.IsNotNullOrWhiteSpace(providerId, nameof(providerId));

        return providerId switch
        {
            SignInProviderDefaults.AzureADScheme => new AzureADSignInProvider(),
            _ => throw new Exception($"Provider with id {providerId} could not be found."),
        };
    }
}
