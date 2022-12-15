using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using OneBeyond.Studio.Crosscuts.Logging;
using OneBeyond.Studio.Domain.SharedKernel.AmbientContexts;
using OneBeyond.Studio.Obelisk.Application.Services.AmbientContexts;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Commands;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Exceptions;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Queries;
using AmbientContext = OneBeyond.Studio.Obelisk.Application.Services.AmbientContexts.AmbientContext;

namespace OneBeyond.Studio.Obelisk.WebApi.Pages.Account.TFA;

[Authorize]
public sealed class ResetAuthenticatorModel : PageModel
{
    private static readonly ILogger Logger = LogManager.CreateLogger<ResetAuthenticatorModel>();

    private readonly UserContext _userContext;
    private readonly IMediator _mediator;

    public ResetAuthenticatorModel(
        IAmbientContextAccessor<AmbientContext> ambientContextAccessor,
        IMediator mediator)
    {
        EnsureArg.IsNotNull(ambientContextAccessor, nameof(ambientContextAccessor));
        EnsureArg.IsNotNull(mediator, nameof(mediator));

        _userContext = ambientContextAccessor.AmbientContext.GetUserContext();
        _mediator = mediator;
    }

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGet()
    {
        var loginTFASettings = await _mediator.Send(
                new GetTfaSettings(_userContext.UserAuthId))
            .ConfigureAwait(false);

        return loginTFASettings.Is2faEnabled
            ? Page()
            : throw new TfaException("Cannot generate recovery codes for the user because they do not have 2FA enabled.");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _mediator.Send(
                new DisableTfa(_userContext.UserAuthId, disableAuthenticator: true))
            .ConfigureAwait(false);

        Logger.LogInformation(
            "User with ID {loginId} has reset their authentication app key",
            _userContext.UserAuthId);

        StatusMessage = "Your authenticator app key has been reset, you will need to configure your authenticator app using the new key.";

        return RedirectToPage("./EnableAuthenticator");
    }
}
