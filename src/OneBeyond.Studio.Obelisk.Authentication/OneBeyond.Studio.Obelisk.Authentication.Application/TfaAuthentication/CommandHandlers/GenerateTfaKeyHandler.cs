using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Identity;
using OneBeyond.Studio.Core.Mediator;
using OneBeyond.Studio.Crosscuts.Strings;
using OneBeyond.Studio.Obelisk.Authentication.Application.Entities;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Exceptions;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Commands;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.TfaAuthentication.CommandHandlers;

internal sealed class GenerateTfaKeyHandler : IRequestHandler<GenerateTfaKey, TfaKey>
{
    private readonly UserManager<AuthUser> _userManager;

    public GenerateTfaKeyHandler(UserManager<AuthUser> userManager)
    {
        EnsureArg.IsNotNull(userManager, nameof(userManager));

        _userManager = userManager;
    }

    public async Task<TfaKey> Handle(GenerateTfaKey command, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        var identityUser = await _userManager.FindByIdAsync(command.LoginId).ConfigureAwait(false)
            ?? throw new AuthLoginNotFoundException($"Login with id {command.LoginId} not found");

        var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(identityUser).ConfigureAwait(false);

        if (unformattedKey.IsNullOrEmpty())
        {
            await _userManager.ResetAuthenticatorKeyAsync(identityUser);
            unformattedKey = await _userManager.GetAuthenticatorKeyAsync(identityUser).ConfigureAwait(false)
                ?? throw new AuthException("Failed to get authenticator key.");
        }

        var sharedKey = FormatKey(unformattedKey);

        return new TfaKey(command.LoginId, identityUser.Email!, unformattedKey, sharedKey);
    }

    private static string FormatKey(string unformattedKey)
    {
        var result = new StringBuilder();

        var currentPosition = 0;
        while (currentPosition + 4 < unformattedKey.Length)
        {
            result.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(' ');
            currentPosition += 4;
        }
        if (currentPosition < unformattedKey.Length)
        {
            result.Append(unformattedKey.AsSpan(currentPosition));
        }

        return result.ToString().ToLowerInvariant();
    }

}
