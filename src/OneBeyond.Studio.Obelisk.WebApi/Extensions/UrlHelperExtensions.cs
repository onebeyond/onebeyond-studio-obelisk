using Microsoft.AspNetCore.Mvc;

namespace OneBeyond.Studio.Obelisk.WebApi.Extensions;

public static class UrlHelperExtensions
{
    public static string GetReturnUrl(this IUrlHelper urlHelper, string? returnUrl) =>
        !string.IsNullOrEmpty(returnUrl) && urlHelper.IsLocalUrl(returnUrl)
            ? returnUrl
            : urlHelper.Page("/Index")!;
}
