using System;

namespace OneBeyond.Studio.Obelisk.Authentication.Application;

/// <summary>
/// SessionConstants class defines constants for the application, such as authentication cookie name and
/// session duration (used to configure session and timeout warnings)
/// </summary>
public static class SessionConstants
{
    /// <summary>
    /// The name to be used for authentication cookies
    /// CHANGE ME: Rename appropriately for your system
    /// </summary>
    public const string CookieName = "ObeliskTemplateCookie";
    /// <summary>
    /// The session duration. It is used for cookie expiration and session automatic locking out both
    /// </summary>
    public static readonly TimeSpan SessionDuration = TimeSpan.FromMinutes(20);
}
