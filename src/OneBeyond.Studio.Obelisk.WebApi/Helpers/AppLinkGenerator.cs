using System;
using EnsureThat;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OneBeyond.Studio.Obelisk.WebApi.Helpers;

public sealed class AppLinkGenerator
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AppLinkGenerator(
        LinkGenerator linkGenerator,
        IHttpContextAccessor httpContextAccessor)
    {
        EnsureArg.IsNotNull(linkGenerator, nameof(linkGenerator));
        EnsureArg.IsNotNull(httpContextAccessor, nameof(httpContextAccessor));
        EnsureArg.IsNotNull(httpContextAccessor.HttpContext, nameof(httpContextAccessor.HttpContext));

        _linkGenerator = linkGenerator;
        _httpContextAccessor = httpContextAccessor;
    }

    public Uri GetSetPasswordUrl(string loginId, string resetPasswordToken)
    {
        EnsureArg.IsNotNullOrWhiteSpace(loginId, nameof(loginId));
        EnsureArg.IsNotNullOrWhiteSpace(resetPasswordToken, nameof(resetPasswordToken));

        var url = _linkGenerator.GetUriByPage(
            httpContext: _httpContextAccessor.HttpContext!,
            page: "/Account/SetPassword",
            values: new { loginId, code = resetPasswordToken })
            ?? throw new Exception("Failed to generate set user password url.");

        return new Uri(url);
    }

    public Uri GetResetPasswordUrl(string resetPasswordToken, string resetPasswordPageUrl)
    {
        EnsureArg.IsNotNullOrWhiteSpace(resetPasswordToken, nameof(resetPasswordToken));

        //var url = _linkGenerator.GetPathByPage(
        //    page: resetPasswordPageUrl,
        //    values: new { code = resetPasswordToken })
        //    ?? throw new Exception("Failed to generate reset user password url.");

        resetPasswordPageUrl = resetPasswordPageUrl.TrimEnd('/');
        var url = $"{resetPasswordPageUrl}?code={Uri.EscapeDataString(resetPasswordToken)}";

        return new Uri(url);
    }

}
