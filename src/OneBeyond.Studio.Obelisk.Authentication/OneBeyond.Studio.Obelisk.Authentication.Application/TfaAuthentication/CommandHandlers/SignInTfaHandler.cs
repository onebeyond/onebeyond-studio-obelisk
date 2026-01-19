using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Identity;
using OneBeyond.Studio.Core.Mediator;
using OneBeyond.Studio.Obelisk.Authentication.Application.Entities;
using OneBeyond.Studio.Obelisk.Authentication.Domain;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Exceptions;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Commands;
using SignInResult = OneBeyond.Studio.Obelisk.Authentication.Domain.SignInResult;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.TfaAuthentication.CommandHandlers;

internal sealed class SignInTfaHandler : IRequestHandler<SignInTfa, SignInResult>
{
    private readonly SignInManager<AuthUser> _signInManager;

    public SignInTfaHandler(
        SignInManager<AuthUser> signInManager
        )
    {
        EnsureArg.IsNotNull(signInManager, nameof(signInManager));

        _signInManager = signInManager;
    }

    public async Task<SignInResult> Handle(SignInTfa command, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        try
        {
            _ = await _signInManager.GetTwoFactorAuthenticationUserAsync().ConfigureAwait(false)
                ?? throw new AuthLoginNotFoundException("Authenticated user not found");

            var signInResult = await _signInManager.TwoFactorAuthenticatorSignInAsync(command.Code, command.RememberMe, command.RememberClient).ConfigureAwait(false);

            var status = SignInStatus.Success;
            var statusMessage = string.Empty;

            if (!signInResult.Succeeded)
            {
                status = signInResult.IsLockedOut ? SignInStatus.LockedOut : SignInStatus.Failure;
                statusMessage = signInResult.IsLockedOut
                    ? "User account locked out"
                    : "Invalid authenticator code provided for user";
            }

            return new SignInResult(status);
        }
        catch (AuthLoginNotFoundException)
        {
            return new SignInResult(SignInStatus.Failure);
        }
    }
}
