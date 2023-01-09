using System;
using System.Linq;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using OneBeyond.Studio.Crosscuts.Logging;
using OneBeyond.Studio.Application.SharedKernel.AmbientContexts;
using OneBeyond.Studio.Obelisk.Application.Services.AmbientContexts;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Commands;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Exceptions;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Queries;
using AmbientContext = OneBeyond.Studio.Obelisk.Application.Services.AmbientContexts.AmbientContext;

namespace OneBeyond.Studio.Obelisk.WebApi.Pages.Account.TFA;

[Authorize]
public sealed class GenerateRecoveryCodesModel : PageModel
{
    private static readonly ILogger Logger = LogManager.CreateLogger<GenerateRecoveryCodesModel>();

    private readonly UserContext _userContext;
    private readonly IMediator _mediator;

    public GenerateRecoveryCodesModel(
        IAmbientContextAccessor<AmbientContext> ambientContextAccessor,
        IMediator mediator)
    {
        EnsureArg.IsNotNull(ambientContextAccessor, nameof(ambientContextAccessor));
        EnsureArg.IsNotNull(mediator, nameof(mediator));

        _userContext = ambientContextAccessor.AmbientContext.GetUserContext();
        _mediator = mediator;

        RecoveryCodes = Array.Empty<string>();
    }

    [TempData]
    public string[] RecoveryCodes { get; set; }
    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
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
        RecoveryCodes = (await _mediator.Send(
                new GenerateTfaRecoveryCodes(_userContext.UserAuthId))
            .ConfigureAwait(false))
            .ToArray();

        Logger.LogInformation(
            "User with ID {loginId} has generated new 2FA recovery codes",
            _userContext.UserAuthId);

        StatusMessage = "You have generated new recovery codes.";

        return RedirectToPage("./ShowRecoveryCodes");
    }
}
