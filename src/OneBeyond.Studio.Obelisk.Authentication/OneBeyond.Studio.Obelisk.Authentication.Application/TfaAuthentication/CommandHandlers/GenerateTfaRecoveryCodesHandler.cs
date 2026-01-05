using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Identity;
using OneBeyond.Studio.Core.Mediator.Commands;
using OneBeyond.Studio.Obelisk.Authentication.Application.Entities;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Exceptions;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Commands;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Exceptions;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.TfaAuthentication.CommandHandlers;

internal sealed class GenerateTfaRecoveryCodesHandler : ICommandHandler<GenerateTfaRecoveryCodes, IEnumerable<string>>
{
    private readonly UserManager<AuthUser> _userManager;

    public GenerateTfaRecoveryCodesHandler(UserManager<AuthUser> userManager)
    {
        EnsureArg.IsNotNull(userManager, nameof(userManager));

        _userManager = userManager;
    }

    public async Task<IEnumerable<string>> HandleAsync(GenerateTfaRecoveryCodes command, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        var identityUser = await _userManager.FindByIdAsync(command.LoginId).ConfigureAwait(false)
            ?? throw new AuthLoginNotFoundException($"Login with id {command.LoginId} not found.");

        return !identityUser.TwoFactorEnabled
            ? throw new TfaException($"Cannot generate recovery codes for the user as they do not have 2FA enabled.")
            : await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(identityUser, 10).ConfigureAwait(false)
                ?? throw new TfaException("Cannot generate recovery codes for the user.");
    }
}
