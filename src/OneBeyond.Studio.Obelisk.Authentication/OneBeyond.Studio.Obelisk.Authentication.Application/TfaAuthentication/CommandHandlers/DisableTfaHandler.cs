using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OneBeyond.Studio.Obelisk.Authentication.Application.Entities;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Exceptions;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Commands;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.TfaAuthentication.CommandHandlers;

internal sealed class DisableTfaHandler : IRequestHandler<DisableTfa, Unit>
{
    private readonly UserManager<AuthUser> _userManager;
    private readonly SignInManager<AuthUser> _signInManager;

    public DisableTfaHandler(
        UserManager<AuthUser> userManager,
        SignInManager<AuthUser> signInManager
        )
    {
        EnsureArg.IsNotNull(signInManager, nameof(signInManager));
        EnsureArg.IsNotNull(userManager, nameof(userManager));

        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<Unit> Handle(DisableTfa command, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        var identityUser = await _userManager.FindByIdAsync(command.LoginId).ConfigureAwait(false)
            ?? throw new AuthLoginNotFoundException($"Login with id {command.LoginId} not found");

        await _userManager.SetTwoFactorEnabledAsync(identityUser, false).ConfigureAwait(false);

        if (command.DisableAuthenticator)
        {
            await _userManager.ResetAuthenticatorKeyAsync(identityUser).ConfigureAwait(false);
        }

        await _signInManager.RefreshSignInAsync(identityUser).ConfigureAwait(false);

        return Unit.Value;
    }
}
