using System;
using EnsureThat;

namespace OneBeyond.Studio.Obelisk.WebApi.Helpers;

public static class AppLinkGenerator
{
    public static Uri GetSetPasswordUrl(
        string loginId, 
        string resetPasswordToken,
        string setPasswordPageUrl)
    {
        EnsureArg.IsNotNullOrWhiteSpace(setPasswordPageUrl, nameof(setPasswordPageUrl));

        setPasswordPageUrl = setPasswordPageUrl.TrimEnd('/');
        var url = $"{setPasswordPageUrl}/auth/setPassword?loginId={Uri.EscapeDataString(loginId)}&code={Uri.EscapeDataString(resetPasswordToken)}";

        return new Uri(url);
    }

    public static Uri GetResetPasswordUrl(string resetPasswordToken, string resetPasswordPageUrl)
    {
        EnsureArg.IsNotNullOrWhiteSpace(resetPasswordToken, nameof(resetPasswordToken));

        resetPasswordPageUrl = resetPasswordPageUrl.TrimEnd('/');
        var url = $"{resetPasswordPageUrl}/auth/resetPassword?code={Uri.EscapeDataString(resetPasswordToken)}";

        return new Uri(url);
    }

}
