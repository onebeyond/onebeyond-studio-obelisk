namespace OneBeyond.Studio.Obelisk.WebApi.Helpers;

public sealed record ClientApplicationOptions
{
    public string Url { get; init; } = default!;

    public string SetPasswordUrl => $"{UrlTrimmed}/auth/setPassword";
    public string ResetPasswordUrl => $"{UrlTrimmed}/auth/resetPassword";

    private string UrlTrimmed => Url.TrimEnd('/');
}

