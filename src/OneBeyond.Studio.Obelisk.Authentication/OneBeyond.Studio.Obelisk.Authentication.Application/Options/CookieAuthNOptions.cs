namespace OneBeyond.Studio.Obelisk.Authentication.Application.Options;

public sealed record CookieAuthNOptions
{
    /// <summary>
    /// Allows the server to set cookies for cross-site requests
    /// This requires to be true to allow the SPA to store authentication
    /// cookies when SPA and WebApi are not in the same domain
    /// NOTE: SHOULD NEVER BE TRUE ON PRODUCTION ENVIRONMENTS 
    /// AS IT CREATES POTENTIAL SECURITY RISKS
    /// </summary>
    public bool AllowCrossSiteCookies { get; init; }
}
