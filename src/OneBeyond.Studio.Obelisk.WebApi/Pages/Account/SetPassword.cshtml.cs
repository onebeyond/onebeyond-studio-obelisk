using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Exceptions;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Queries;

namespace OneBeyond.Studio.Obelisk.WebApi.Pages.Account;

public class SetPasswordModel : PageModel
{
    private readonly IMediator _mediator;

    public SetPasswordModel(
        IMediator mediator)
    {
        EnsureArg.IsNotNull(mediator, nameof(mediator));

        _mediator = mediator;

        Input = new InputModel();
    }

    [BindProperty]
    public InputModel Input { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    public class InputModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; } = default!;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = default!;

        public string Code { get; set; } = default!;

        public string LoginId { get; set; } = default!;

    }

    public async Task<IActionResult> OnGetAsync(
        string? code = default,
        string? loginId = default,
        CancellationToken cancellationToken = default
        )
    {
        if (code == null || loginId == null)
        {
            return Page();
        }

        var hasPassword = await _mediator.Send(
            new CheckLoginHasPassword(loginId), cancellationToken)
            .ConfigureAwait(false);

        if (hasPassword)
        {
            return RedirectToPage("./AccountAlreadySet");
        }

        Input = new InputModel
        {
            Code = code,
            LoginId = loginId
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(
        CancellationToken cancellationToken = default
        )
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            await _mediator.Send(
                new SetPassword(Input.LoginId!, Input.Code, Input.NewPassword), cancellationToken)
                .ConfigureAwait(false);
        }
        catch (AuthLoginNotFoundException)
        {
            return RedirectToPage("./ResetPasswordConfirmation"); // Don't reveal that the user does not exist
        }
        catch (AuthException authException)
        {
            ModelState.AddModelError(string.Empty, authException.Message);
            return Page();
        }

        return RedirectToPage("./ResetPasswordConfirmation");
    }
}
