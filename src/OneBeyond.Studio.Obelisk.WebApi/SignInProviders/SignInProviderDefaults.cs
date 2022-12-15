using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace OneBeyond.Studio.Obelisk.WebApi.SignInProviders;

public static class SignInProviderDefaults
{
    public const string LoginProviderKey = "LoginProvider";
    public const string AzureADScheme = OpenIdConnectDefaults.AuthenticationScheme;
}
