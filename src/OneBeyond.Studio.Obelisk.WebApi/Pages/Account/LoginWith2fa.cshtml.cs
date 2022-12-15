using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using OneBeyond.Studio.Obelisk.Authentication.Domain;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Commands;
using OneBeyond.Studio.Obelisk.WebApi.Extensions;
using OneBeyond.Studio.Obelisk.WebApi.Localizations;

namespace OneBeyond.Studio.Obelisk.WebApi.Pages.Account;

public sealed class LoginWith2faModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public LoginWith2faModel(
        IMediator mediator,
        IStringLocalizer<SharedResources> localizer
        )
    {
        EnsureArg.IsNotNull(mediator, nameof(mediator));
        EnsureArg.IsNotNull(localizer, nameof(localizer));

        _mediator = mediator;
        _localizer = localizer;

        Input = new InputModel();
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }

    public class InputModel
    {
        [Required]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Authenticator code")]
        public string TwoFactorCode { get; set; } = default!;

        [Display(Name = "Remember this machine")]
        public bool RememberMachine { get; set; }
    }

    public IActionResult OnGetAsync(bool rememberMe, string? returnUrl = default)
    {
        ReturnUrl = returnUrl;
        RememberMe = rememberMe;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(
        bool rememberMe,
        string? returnUrl = default,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var authenticatorCode = Input.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

        var result = await _mediator.Send(
            new SignInTfa(
                authenticatorCode,
                rememberMe,
                Input.RememberMachine),
            cancellationToken)
            .ConfigureAwait(false);

        switch (result.Status)
        {
            case SignInStatus.Success:
                return LocalRedirect(Url.GetReturnUrl(returnUrl));
            case SignInStatus.LockedOut:
                return RedirectToPage("./Lockout");
            case SignInStatus.Failure:
            case SignInStatus.UnknownUser:
                ModelState.AddModelError(string.Empty, _localizer[result.StatusMessage].Value);
                break;
            default:
                ModelState.AddModelError(string.Empty, _localizer["Unknown Sign-in status."].Value);
                break;
        }

        return Page();
    }
}
