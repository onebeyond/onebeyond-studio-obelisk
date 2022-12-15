using System;
using System.Threading;
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
using OneBeyond.Studio.Obelisk.Authentication.Domain.AuthenticationFlows;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Commands;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Exceptions;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Queries;
using AmbientContext = OneBeyond.Studio.Obelisk.Application.Services.AmbientContexts.AmbientContext;

namespace OneBeyond.Studio.Obelisk.WebApi.Pages.Account.TFA;

[Authorize]
public sealed class Disable2faModel : PageModel
{
    private static readonly ILogger Logger = LogManager.CreateLogger<Disable2faModel>();

    private readonly IAuthenticationFlowHandler _authenticationFlow;
    private readonly UserContext _userContext;
    private readonly IMediator _mediator;

    public Disable2faModel(
        IAuthenticationFlowHandler authenticationFlow,
        IAmbientContextAccessor<AmbientContext> ambientContextAccessor,
        IMediator mediator)
    {
        EnsureArg.IsNotNull(authenticationFlow, nameof(authenticationFlow));
        EnsureArg.IsNotNull(ambientContextAccessor, nameof(ambientContextAccessor));
        EnsureArg.IsNotNull(mediator, nameof(mediator));

        _authenticationFlow = authenticationFlow;
        _userContext = ambientContextAccessor.AmbientContext.GetUserContext();
        _mediator = mediator;
    }

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var loginTFASettings = await _mediator.Send(
                new GetTfaSettings(_userContext.UserAuthId),
                cancellationToken)
            .ConfigureAwait(false);

        if (!loginTFASettings.Is2faEnabled)
        {
            throw new TfaException($"Cannot disable 2FA for the user because they do not have 2FA enabled.");
        }

        var is2faMandatory = await _authenticationFlow.IsTwoFactorAthenticationRequiredForLoginAsync(
                _userContext.UserAuthId,
                cancellationToken)
            .ConfigureAwait(false);

        return is2faMandatory
            ? throw new InvalidOperationException($"Cannot disable 2FA for the user as it's mandatory.")
            : Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _mediator.Send(
                new DisableTfa(_userContext.UserAuthId, disableAuthenticator: false))
            .ConfigureAwait(false);

        Logger.LogInformation(
            "User with ID '{loginId}' has disabled 2fa",
            _userContext.UserAuthId);

        StatusMessage = "2fa has been disabled. You can reenable 2fa when you setup an authenticator app";

        return RedirectToPage("./TfaAuthentication");
    }
}
