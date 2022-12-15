using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OneBeyond.Studio.Obelisk.Authentication.Application.Entities;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Exceptions;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Commands;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Exceptions;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.TfaAuthentication.CommandHandlers;

internal sealed class EnableTfaHandler : IRequestHandler<EnableTfa, Unit>
{
    private readonly UserManager<AuthUser> _userManager;

    public EnableTfaHandler(
        UserManager<AuthUser> userManager
        )
    {
        EnsureArg.IsNotNull(userManager, nameof(userManager));

        _userManager = userManager;
    }

    public async Task<Unit> Handle(EnableTfa command, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        var identityUser = await _userManager.FindByIdAsync(command.LoginId).ConfigureAwait(false)
            ?? throw new AuthLoginNotFoundException($"Login with id {command.LoginId} not found");

        var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
            identityUser,
            _userManager.Options.Tokens.AuthenticatorTokenProvider,
            command.VerificationCode).ConfigureAwait(false);

        if (!is2faTokenValid)
        {
            throw new InvalidTfaTokenException("Verification code is invalid.");
        }

        await _userManager.SetTwoFactorEnabledAsync(identityUser, true).ConfigureAwait(false);

        return Unit.Value;
    }
}
