using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OneBeyond.Studio.Crosscuts.Logging;
using OneBeyond.Studio.Domain.SharedKernel.AmbientContexts;
using OneBeyond.Studio.Obelisk.Application.Services.AmbientContexts;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Commands;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Exceptions;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Queries;
using OneBeyond.Studio.Obelisk.WebApi.Extensions;
using OneBeyond.Studio.Obelisk.WebApi.Localizations;
using AmbientContext = OneBeyond.Studio.Obelisk.Application.Services.AmbientContexts.AmbientContext;

namespace OneBeyond.Studio.Obelisk.WebApi.Pages.Account.TFA;

[Authorize]
public sealed class EnableAuthenticatorModel : PageModel
{
    private static readonly ILogger Logger = LogManager.CreateLogger<EnableAuthenticatorModel>();

    private readonly UserContext _userContext;
    private readonly IMediator _mediator;
    private readonly UrlEncoder _urlEncoder;
    private readonly IStringLocalizer<SharedResources> _localizer;

    private const string AUTHENTICATOR_URI_FORMAT = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

    public EnableAuthenticatorModel(
        IAmbientContextAccessor<AmbientContext> ambientContextAccessor,
        IMediator mediator,
        UrlEncoder urlEncoder,
        IStringLocalizer<SharedResources> localizer)
    {
        EnsureArg.IsNotNull(ambientContextAccessor, nameof(ambientContextAccessor));
        EnsureArg.IsNotNull(mediator, nameof(mediator));
        EnsureArg.IsNotNull(urlEncoder, nameof(urlEncoder));
        EnsureArg.IsNotNull(localizer, nameof(localizer));

        _userContext = ambientContextAccessor.AmbientContext.GetUserContext();
        _mediator = mediator;
        _urlEncoder = urlEncoder;
        _localizer = localizer;

        Input = new InputModel();

        RecoveryCodes = Array.Empty<string>();
    }

    public string? SharedKey { get; set; }
    public string? ReturnUrl { get; set; }
    public string? AuthenticatorUri { get; set; }
    [TempData]
    public string[] RecoveryCodes { get; set; }
    [TempData]
    public string? StatusMessage { get; set; }
    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        [Required]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Verification Code")]
        public string Code { get; set; } = default!;
    }

    public async Task<IActionResult> OnGetAsync(string? returnUrl = default)
    {
        await LoadSharedKeyAndQrCodeUriAsync().ConfigureAwait(false);

        ReturnUrl = returnUrl;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = default)
    {
        ReturnUrl = returnUrl;

        if (!ModelState.IsValid)
        {
            await LoadSharedKeyAndQrCodeUriAsync().ConfigureAwait(false);
            return Page();
        }

        var verificationCode = Input.Code.Replace(" ", string.Empty).Replace("-", string.Empty); // Strip spaces and hypens

        try
        {
            await _mediator.Send(
                    new EnableTfa(_userContext.UserAuthId, verificationCode))
                .ConfigureAwait(false);

            Logger.LogInformation(
                "User with ID '{UserId}' has enabled 2FA with an authenticator app.",
                _userContext.UserAuthId);

            StatusMessage = _localizer["Your authenticator app has been verified."].Value;

            var loginTFASettings = await _mediator.Send(
                    new GetTfaSettings(_userContext.UserAuthId))
                .ConfigureAwait(false);

            if (loginTFASettings.RecoveryCodesLeft == 0)
            {
                RecoveryCodes = (await _mediator.Send(
                        new GenerateTfaRecoveryCodes(_userContext.UserAuthId))
                    .ConfigureAwait(false))
                    .ToArray();
                return RedirectToPage("./ShowRecoveryCodes");
            }
            else
            {
                Logger.LogInformation("User {LoginId} logged in", _userContext.UserAuthId);
                return LocalRedirect(Url.GetReturnUrl(returnUrl));
            }
        }
        catch (InvalidTfaTokenException)
        {
            ModelState.AddModelError("Input.Code", _localizer["Verification code is invalid."].Value);
            await LoadSharedKeyAndQrCodeUriAsync().ConfigureAwait(false);
            return Page();
        }
    }

    private async Task LoadSharedKeyAndQrCodeUriAsync()
    {
        var authenticatorKey = await _mediator.Send(
                new GenerateTfaKey(_userContext.UserAuthId))
            .ConfigureAwait(false);

        SharedKey = authenticatorKey.SharedKey;

        AuthenticatorUri = GenerateQrCodeUri(authenticatorKey.Email, authenticatorKey.RawKey);
    }

    private string GenerateQrCodeUri(string email, string unformattedKey)
        => string.Format(
            AUTHENTICATOR_URI_FORMAT,
            _urlEncoder.Encode("WaterlooTemplate"),
            _urlEncoder.Encode(email),
            unformattedKey);
}
