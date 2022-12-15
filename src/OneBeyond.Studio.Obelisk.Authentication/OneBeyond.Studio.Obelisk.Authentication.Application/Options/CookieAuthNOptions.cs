namespace OneBeyond.Studio.Obelisk.Authentication.Application.Options;

public sealed record CookieAuthNOptions
{
    /// <summary>
    /// Disables redirects when handles Challenge/Forbid for a request
    /// </summary>
    public bool DisableAuthNRedirects { get; init; }

    /// <summary>
    /// Allows the server to set cookies for cross-site requests
    /// This requires to be true to allow the SPA to store authentication
    /// cookies when SPA and WebApi are not in the same domain
    /// NOTE: should never be true in Production environments
    /// </summary>
    public bool AllowCrossSiteCookies { get; init; }
}
