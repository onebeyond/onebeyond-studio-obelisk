using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OneBeyond.Studio.Crosscuts.Logging;
using OneBeyond.Studio.Obelisk.Authentication.Domain;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Commands;
using OneBeyond.Studio.Obelisk.WebApi.Localizations;

namespace OneBeyond.Studio.Obelisk.WebApi.Pages.Account;

public sealed class LoginWithRecoveryCodeModel : PageModel
{
    private static readonly ILogger Logger = LogManager.CreateLogger<LoginWithRecoveryCodeModel>();

    private readonly IMediator _mediator;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public LoginWithRecoveryCodeModel(
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

    public string? ReturnUrl { get; private set; }

    public sealed class InputModel
    {
        [BindProperty]
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Recovery Code")]
        public string RecoveryCode { get; set; } = default!;
    }

    public IActionResult OnGet(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var recoveryCode = Input.RecoveryCode.Replace(" ", string.Empty);

        var result = await _mediator.Send(
            new SignInTfaWithRecoveryCode(recoveryCode),
            cancellationToken)
            .ConfigureAwait(false);

        if (result.Status == SignInStatus.Success)
        {
            Logger.LogInformation($"User with ID {result.LoginId} logged in with a recovery code.");
            return LocalRedirect(returnUrl ?? Url.Content("~/"));
        }
        if (result.Status == SignInStatus.LockedOut)
        {
            Logger.LogWarning($"User with ID {result.LoginId} account locked out.");
            return RedirectToPage("./Lockout");
        }
        else
        {
            Logger.LogWarning($"Invalid recovery code entered for user with ID {result.LoginId} ");
            ModelState.AddModelError(string.Empty, _localizer["Invalid recovery code entered."].Value);
            return Page();
        }
    }
}
