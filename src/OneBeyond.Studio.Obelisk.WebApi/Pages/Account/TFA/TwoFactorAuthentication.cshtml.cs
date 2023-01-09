using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OneBeyond.Studio.Application.SharedKernel.AmbientContexts;
using OneBeyond.Studio.Obelisk.Application.Services.AmbientContexts;
using OneBeyond.Studio.Obelisk.Authentication.Domain.AuthenticationFlows;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Commands;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Queries;
using AmbientContext = OneBeyond.Studio.Obelisk.Application.Services.AmbientContexts.AmbientContext;

namespace OneBeyond.Studio.Obelisk.WebApi.Pages.Account.TFA;

[Authorize]
public sealed class TwoFactorAuthenticationModel : PageModel
{
    private readonly IAuthenticationFlowHandler _authenticationFlow;
    private readonly UserContext _userContext;
    private readonly IMediator _mediator;

    public TwoFactorAuthenticationModel(
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

    public bool HasAuthenticator { get; private set; }
    public int RecoveryCodesLeft { get; private set; }
    public bool Is2faEnabled { get; private set; }
    public bool Is2faMandatory { get; private set; }
    public bool IsMachineRemembered { get; private set; }
    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        Is2faMandatory = await _authenticationFlow.IsTwoFactorAthenticationRequiredForLoginAsync(
                _userContext.UserAuthId,
                cancellationToken)
            .ConfigureAwait(false);

        var loginTFASettings = await _mediator.Send(
                new GetTfaSettings(_userContext.UserAuthId),
                cancellationToken)
            .ConfigureAwait(false);

        HasAuthenticator = loginTFASettings.HasAuthenticator;
        Is2faEnabled = loginTFASettings.Is2faEnabled;
        IsMachineRemembered = loginTFASettings.IsMachineRemembered;
        RecoveryCodesLeft = loginTFASettings.RecoveryCodesLeft;

        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        await _mediator.Send(
                new ForgetTfaClient(_userContext.UserAuthId))
            .ConfigureAwait(false);

        StatusMessage = "The current browser has been forgotten. When you login again from this browser you will be prompted for your 2fa code.";

        return RedirectToPage();
    }
}
