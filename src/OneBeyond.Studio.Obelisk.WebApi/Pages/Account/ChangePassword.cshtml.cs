using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using OneBeyond.Studio.Domain.SharedKernel.AmbientContexts;
using OneBeyond.Studio.Obelisk.Application.Services.AmbientContexts;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Exceptions;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Queries;
using OneBeyond.Studio.Obelisk.WebApi.Localizations;
using AmbientContext = OneBeyond.Studio.Obelisk.Application.Services.AmbientContexts.AmbientContext;

namespace OneBeyond.Studio.Obelisk.WebApi.Pages.Account;

[Authorize]
public sealed class ChangePasswordModel : PageModel
{
    private readonly UserContext _userContext;
    private readonly IMediator _mediator;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public ChangePasswordModel(
        IAmbientContextAccessor<AmbientContext> ambientContextAccessor,
        IMediator mediator,
        IStringLocalizer<SharedResources> localizer)
    {
        EnsureArg.IsNotNull(ambientContextAccessor, nameof(ambientContextAccessor));
        EnsureArg.IsNotNull(mediator, nameof(mediator));
        EnsureArg.IsNotNull(localizer, nameof(localizer));

        _userContext = ambientContextAccessor.AmbientContext.GetUserContext();
        _mediator = mediator;
        _localizer = localizer;

        Input = new InputModel();
    }

    [BindProperty]
    public InputModel Input { get; set; }
    [TempData]
    public string? StatusMessage { get; set; }

    public class InputModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Old Password")]
        public string OldPassword { get; set; } = default!;
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 10)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = default!;
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = default!;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var hasPassword = await _mediator.Send(
                new CheckLoginHasPassword(_userContext.UserAuthId))
            .ConfigureAwait(false);

        return !hasPassword
            ? RedirectToPage("./SetPassword")
            : Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            await _mediator.Send(
                    new ChangePassword(
                        _userContext.UserAuthId,
                        Input.OldPassword,
                        Input.NewPassword),
                    cancellationToken)
                .ConfigureAwait(false);
        }
        catch (AuthException authExpection)
        {
            ModelState.AddModelError(string.Empty, authExpection.Message);
            return Page();
        }

        StatusMessage = _localizer["Your password has been changed."].Value;

        return RedirectToPage();
    }
}
