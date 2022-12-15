using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Exceptions;

namespace OneBeyond.Studio.Obelisk.WebApi.Pages.Account;

public class ResetPasswordModel : PageModel
{
    private readonly IMediator _mediator;

    public ResetPasswordModel(IMediator mediator)
    {
        EnsureArg.IsNotNull(mediator, nameof(mediator));
        _mediator = mediator;

        Input = new InputModel();
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        [Required]
        public string UserName { get; set; } = default!;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }

        public string? Code { get; set; }
    }

    public IActionResult OnGet(string? code = null)
    {
        if (code == null)
        {
            return Page();
        }

        Input = new InputModel
        {
            Code = code
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid || Input.Code == null)
        {
            return Page();
        }

        try
        {
            await _mediator.Send(
                new ResetPassword(
                    Input.UserName,
                    Input.Code,
                    Input.Password),
                cancellationToken)
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
