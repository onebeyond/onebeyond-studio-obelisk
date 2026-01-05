using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Identity;
using OneBeyond.Studio.Core.Mediator.Queries;
using OneBeyond.Studio.Obelisk.Authentication.Application.Entities;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Exceptions;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Queries;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.TfaAuthentication.QueryHandlers;

internal sealed class GetTfaSettingsHandler : IQueryHandler<GetTfaSettings, LoginTfaSettings>
{
    private readonly UserManager<AuthUser> _userManager;
    private readonly SignInManager<AuthUser> _signInManager;

    public GetTfaSettingsHandler(
        UserManager<AuthUser> userManager,
        SignInManager<AuthUser> signInManager
    )
    {
        EnsureArg.IsNotNull(userManager, nameof(userManager));
        EnsureArg.IsNotNull(signInManager, nameof(signInManager));

        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<LoginTfaSettings> HandleAsync(GetTfaSettings query, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(query, nameof(query));

        var identityUser = await _userManager.FindByIdAsync(query.LoginId).ConfigureAwait(false)
            ?? throw new AuthLoginNotFoundException($"Login with Id {query.LoginId} not found");

        var hasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(identityUser) != null;
        var is2faEnabled = await _userManager.GetTwoFactorEnabledAsync(identityUser);
        var isMachineRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(identityUser);
        var recoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(identityUser);

        return new LoginTfaSettings(hasAuthenticator, is2faEnabled, isMachineRemembered, recoveryCodesLeft);
    }
}
