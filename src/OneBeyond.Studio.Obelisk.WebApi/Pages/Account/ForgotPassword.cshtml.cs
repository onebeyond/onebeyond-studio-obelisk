using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Exceptions;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Commands;
using OneBeyond.Studio.Obelisk.WebApi.Helpers;
using OneBeyond.Studio.Obelisk.WebApi.Localizations;

namespace OneBeyond.Studio.Obelisk.WebApi.Pages.Account;

public class ForgotPasswordModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly AppLinkGenerator _linkGenerator;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public ForgotPasswordModel(
        IMediator mediator,
        AppLinkGenerator linkGenerator,
        IStringLocalizer<SharedResources> localizer
        )
    {
        EnsureArg.IsNotNull(mediator, nameof(mediator));
        EnsureArg.IsNotNull(linkGenerator, nameof(linkGenerator));
        EnsureArg.IsNotNull(localizer, nameof(localizer));

        _mediator = mediator;
        _linkGenerator = linkGenerator;
        _localizer = localizer;

        Input = new InputModel();
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public string? ErrorMessage { get; set; }

    public sealed class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var resetPasswordTokenResult = await _mediator.Send(
                new GenerateResetPasswordTokenByEmail(Input.Email), cancellationToken)
                .ConfigureAwait(false);

            await _mediator.Send(
                new SendResetPasswordEmail(
                    resetPasswordTokenResult.LoginId,
                    _linkGenerator.GetResetPasswordUrl(resetPasswordTokenResult.Value)),
                cancellationToken)
                .ConfigureAwait(false);

            return RedirectToPage("./ForgotPasswordConfirmation");
        }
        catch (AuthLoginNotFoundException)
        {
            // Don't reveal that the user does not exist
            return RedirectToPage("./ForgotPasswordConfirmation");
        }
        catch (Exception)
        {
            ErrorMessage = _localizer["Unable to generate forgotten password email. Please contact your system administrator for assistance."].Value;
            return Page();
        }
    }
}
