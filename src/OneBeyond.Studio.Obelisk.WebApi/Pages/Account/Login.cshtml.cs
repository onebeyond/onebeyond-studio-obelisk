using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OneBeyond.Studio.Crosscuts.Exceptions;
using OneBeyond.Studio.Crosscuts.Logging;
using OneBeyond.Studio.Crosscuts.Strings;
using OneBeyond.Studio.Obelisk.Authentication.Domain;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Exceptions;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Queries;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Commands;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Entities;
using OneBeyond.Studio.Obelisk.WebApi.Extensions;
using OneBeyond.Studio.Obelisk.WebApi.Localizations;
using OneBeyond.Studio.Obelisk.WebApi.SignInProviders;

namespace OneBeyond.Studio.Obelisk.WebApi.Pages.Account;

public class LoginModel : PageModel
{
    private static readonly ILogger Logger = LogManager.CreateLogger<LoginModel>();

    private readonly IMediator _mediator;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public LoginModel(IMediator mediator,
        IStringLocalizer<SharedResources> localizer)
    {
        EnsureArg.IsNotNull(mediator, nameof(mediator));
        EnsureArg.IsNotNull(localizer, nameof(localizer));

        _mediator = mediator;

        _localizer = localizer;

        Input = new InputModel();
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public string? ReturnUrl { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync(string? returnUrl = default)
    {
        if (!ErrorMessage.IsNullOrEmpty())
        {
            ModelState.AddModelError(string.Empty, ErrorMessage);
        }

        // Clear the existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = default)
    {
        ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl;

        if (ModelState.IsValid)
        {
            var result = await _mediator.Send(new SignInViaPassword(Input.UserName, Input.Password, Input.RememberMe, true)).ConfigureAwait(false);

            switch (result.Status)
            {
                case SignInStatus.Success:
                    return LocalRedirect(Url.GetReturnUrl(returnUrl));
                case SignInStatus.LockedOut:
                    return RedirectToPage("./Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, Input.RememberMe });
                case SignInStatus.Failure:
                case SignInStatus.UnknownUser:
                    ModelState.AddModelError(string.Empty, _localizer[result.StatusMessage].Value);
                    break;
                case SignInStatus.ConfigureTFA:
                    return RedirectToPage("./TFA/EnableAuthenticator", new { ReturnUrl = returnUrl });
                default:
                    ModelState.AddModelError(string.Empty, _localizer["Unknown Sign-in status."].Value);
                    break;
            }
        }

        return Page();
    }

    public IActionResult OnGetLoginViaProvider(
        string signInProviderId,
        Uri? returnUrl = default)
    {
        var completeExternalLoginUrl = Url.Page(
            "Login",
            "LoggedInViaProvider",
            new
            {
                signInProviderId,
                returnUrl
            });

        //Authentication properties must be configured as below
        //Otherwise GetExternalLoginInformation will fail
        //https://github.com/aspnet/Identity/blob/feedcb5c53444f716ef5121d3add56e11c7b71e5/src/Identity/SignInManager.cs#L666
        var properties = new AuthenticationProperties { RedirectUri = completeExternalLoginUrl };
        properties.Items[SignInProviderDefaults.LoginProviderKey] = signInProviderId;

        return Challenge(
            properties,
            signInProviderId);
    }

    public async Task<IActionResult> OnGetLoggedInViaProviderAsync(
        string signInProviderId,
        Uri? returnUrl = default,
        string? remoteError = default,
        CancellationToken cancellationToken = default)
    {
        EnsureArg.IsNotNullOrWhiteSpace(signInProviderId, nameof(signInProviderId));

        if (remoteError != null)
        {
            Logger.LogError("Error from external provider: {RemoteError}", remoteError);
            return Page();
        }

        try
        {
            var externalLoginInfo = await _mediator.Send(
                    new GetExternalLoginInformation(),
                    cancellationToken)
                .ConfigureAwait(false);

            var signInProvider = SignInProviderFactory.GetSignInProvider(signInProviderId);

            var externalLogin = new SignInProviderLogin(
                    signInProviderId,
                    externalLoginInfo.ProviderKey,
                    externalLoginInfo.DisplayName);

            var loginId = await _mediator.Send(
                    new GetLoginForProviderLogin(externalLogin),
                    cancellationToken)
                .ConfigureAwait(false);

            if (externalLoginInfo.Principal.Identity is null)
            {
                Logger.LogError("Identity was null when signing in using {LoginProvider}", signInProviderId);
                return Page();
            }

            if (loginId == null)
            {
                var email = signInProvider.GetEmail(externalLoginInfo.Principal.Identity);

                //In our system we do not have any logins/users yet associated with this external login.
                //What happens here depends on the business model. In our case we automatically create login and a user
                //in our system. By default this user is assigned to UserRole.USER role
                var newLogin = await _mediator.Send(
                    new CreateLogin(
                        email,
                        email,
                        UserRole.USER,
                        externalLogin
                    ),
                    cancellationToken).ConfigureAwait(false);

                await _mediator.Send(new CreateUser(
                    newLogin.LoginId,
                    email,
                    email,
                    UserRole.USER,
                    null //we do not provide reset password feature for externally signed it users. 
                ),
                cancellationToken).ConfigureAwait(false);
            }

            var signInResult = await _mediator.Send(
                    new SignInViaProvider(externalLogin),
                    cancellationToken)
                .ConfigureAwait(false);

            switch (signInResult.Status)
            {
                case SignInStatus.Success:
                    return LocalRedirect(
                        Url.GetReturnUrl(returnUrl?.OriginalString));
                case SignInStatus.LockedOut:
                    return RedirectToPage(
                        "./Lockout");
                case SignInStatus.Failure:
                case SignInStatus.UnknownUser:
                    ModelState.AddModelError(string.Empty, _localizer[signInResult.StatusMessage].Value);
                    return Page();
                default:
                    ModelState.AddModelError(string.Empty, _localizer["Unknown Sign-in status."].Value);
                    return Page();
            }
        }
        catch (AuthLoginNotFoundException exception)
        when (!exception.IsCritical())
        {
            Logger.LogWarning(
                exception,
                "Unable to find a user while signing in. Return them back to the login page");
            return Page(); // Do not unveil user is not found
        }
        catch (Exception exception)
        when (!exception.IsCritical())
        {
            Logger.LogError(
                exception,
                "Unable to proceed with user sign-in due to unexpected error");
            ModelState.AddModelError(string.Empty, _localizer["Unable to login. Please contact your support team for assistance."].Value);
            return Page();
        }
    }

    public string GetLoginViaAzureADUrl()
        => Url.Page(
            "Login",
            "LoginViaProvider",
            new
            {
                signInProviderId = SignInProviderDefaults.AzureADScheme,
                returnUrl = ReturnUrl
            })!;

    public sealed class InputModel
    {
        [Required(ErrorMessage = "The username field is required.")]
        public string UserName { get; set; } = default!;
        [Required(ErrorMessage = "The password field is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
