using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Identity;
using OneBeyond.Studio.Core.Mediator.Commands;
using OneBeyond.Studio.Obelisk.Authentication.Application.Entities;
using OneBeyond.Studio.Obelisk.Authentication.Domain;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Exceptions;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Commands;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.TfaAuthentication.CommandHandlers;

internal sealed class SignInTfaWithRecoveryCodeHandler : ICommandHandler<SignInTfaWithRecoveryCode, SignInWithRecoveryCodeResult>
{
    private readonly SignInManager<AuthUser> _signInManager;

    public SignInTfaWithRecoveryCodeHandler(
        SignInManager<AuthUser> signInManager
        )
    {
        EnsureArg.IsNotNull(signInManager, nameof(signInManager));

        _signInManager = signInManager;
    }

    public async Task<SignInWithRecoveryCodeResult> HandleAsync(SignInTfaWithRecoveryCode command, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        try
        {
            var identityUser = await _signInManager.GetTwoFactorAuthenticationUserAsync().ConfigureAwait(false)
                ?? throw new AuthLoginNotFoundException("Authenticated user not found");

            var signInResult = await _signInManager.TwoFactorRecoveryCodeSignInAsync(command.RecoveryCode).ConfigureAwait(false);

            var status = SignInStatus.Success;
            var statusMessage = string.Empty;

            if (!signInResult.Succeeded)
            {
                status = signInResult.IsLockedOut ? SignInStatus.LockedOut : SignInStatus.Failure;
                statusMessage = signInResult.IsLockedOut
                    ? "User account locked out"
                    : "Invalid authenticator code provided for user";
            }

            return new SignInWithRecoveryCodeResult(identityUser.Id, status, statusMessage);
        }
        catch (AuthLoginNotFoundException)
        {
            return new SignInWithRecoveryCodeResult(string.Empty, SignInStatus.Failure, "Invalid username or password");
        }
    }
}
