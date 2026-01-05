using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using OneBeyond.Studio.Core.Mediator.Commands;
using OneBeyond.Studio.Crosscuts.Logging;
using OneBeyond.Studio.Obelisk.Authentication.Application.Entities;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Exceptions;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Commands;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.TfaAuthentication.CommandHandlers;

internal sealed class DisableTfaHandler : ICommandHandler<DisableTfa, bool>
{
    private readonly UserManager<AuthUser> _userManager;
    private readonly SignInManager<AuthUser> _signInManager;
    private readonly ILogger _logger = LogManager.CreateLogger<DisableTfaHandler>();

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

    public async Task<bool> HandleAsync(DisableTfa command, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        var identityUser = await _userManager.FindByIdAsync(command.LoginId).ConfigureAwait(false)
            ?? throw new AuthLoginNotFoundException($"Login with id {command.LoginId} not found");

        var result = await _userManager.SetTwoFactorEnabledAsync(identityUser, false).ConfigureAwait(false);

        if (!result.Succeeded)
        {
            _logger.LogWarning(string.Join(", ", result.Errors.Select(x => $"{x.Code} - {x.Description}")));
        }

        if (command.DisableAuthenticator)
        {
            result = await _userManager.ResetAuthenticatorKeyAsync(identityUser).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                _logger.LogWarning(string.Join(", ", result.Errors.Select(x => $"{x.Code} - {x.Description}")));
            }
        }

        await _signInManager.RefreshSignInAsync(identityUser).ConfigureAwait(false);

        return result.Succeeded;
    }
}
