using System;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using OneBeyond.Studio.Crosscuts.Exceptions;
using OneBeyond.Studio.Crosscuts.Logging;
using OneBeyond.Studio.Crosscuts.Strings;
using OneBeyond.Studio.Crosscuts.Utilities.Identities;
using OneBeyond.Studio.Obelisk.Authentication.Domain.AuthenticationFlows;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.Services.AuthenticationFlows;

internal sealed class CookieAuthenticationFlow : CookieAuthenticationEvents
{
    private static readonly ILogger Logger = LogManager.CreateLogger<CookieAuthenticationFlow>();

    private readonly IAuthenticationFlowHandler _authFlowHandler;

    public CookieAuthenticationFlow(
        IAuthenticationFlowHandler authFlowHandler)
    {
        EnsureArg.IsNotNull(authFlowHandler, nameof(authFlowHandler));

        _authFlowHandler = authFlowHandler;
    }

    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        var loginId = context.Principal?.Identity?.TryGetLoginId() ?? string.Empty;

        if (loginId.IsNullOrWhiteSpace())
        {
            Logger.LogWarning(
                "Unable to find login for principal {PrincipalIdentityName}. Reject the principal",
                context.Principal?.Identity?.Name);
            context.RejectPrincipal();
            await context.HttpContext.SignOutAsync();
            return;
        }

        try
        {
            await SecurityStampValidator.ValidatePrincipalAsync(context); //We validate security stamp to make sure that after a user is logged out, all existing cookies are invalid.
            await _authFlowHandler.OnValidatingLoginAsync(loginId, CancellationToken.None);
            await base.ValidatePrincipal(context);
            Logger.LogInformation(
                "Succeeded to validate login {LoginId} for principal {PrincipalIdentityName}",
                loginId,
                context.Principal?.Identity?.Name);
        }
        catch (Exception exception)
        when (!exception.IsCritical())
        {
            Logger.LogWarning(
                exception,
                "Failed to validate login {LoginId} for principal {PrincipalIdentityName}. Reject the principal",
                loginId,
                context.Principal?.Identity?.Name);
            context.RejectPrincipal();
            await context.HttpContext.SignOutAsync();
        }
    }

    public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
    {
        //The default Cookie authentication logic redirects to login page in case if a request is not authorized.
        //As we no longer have a login page in Web API, we just return Http status code Status401Unauthorized
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    }

    public override Task RedirectToAccessDenied(RedirectContext<CookieAuthenticationOptions> context)
    {
        //The default Cookie authentication logic redirects to login page in case if access denied.
        //As we no longer have a login page in Web API, we just return Http status code Status401Unauthorized
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    }
}
