namespace OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication;

public sealed record TfaAuthenticatorSettings
{
    public string SharedKey { get; init; } = default!;
    public string AuthenticationUri { get; init; } = default!;
}
