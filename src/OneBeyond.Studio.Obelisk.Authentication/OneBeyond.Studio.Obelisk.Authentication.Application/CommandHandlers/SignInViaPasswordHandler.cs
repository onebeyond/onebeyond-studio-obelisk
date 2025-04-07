using System;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OneBeyond.Studio.Crosscuts.Logging;
using OneBeyond.Studio.Obelisk.Authentication.Application.Entities;
using OneBeyond.Studio.Obelisk.Authentication.Domain;
using OneBeyond.Studio.Obelisk.Authentication.Domain.AuthenticationFlows;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.CommandHandlers;

[Obsolete("This relies on cross-origin cookies, which are now deprecated in some browsers")]
internal sealed class SignInViaPasswordHandler : SignInHandler<SignInViaPassword>
{
    private readonly UserManager<AuthUser> _userManager;
    private static readonly ILogger Logger = LogManager.CreateLogger<SignInViaPasswordHandler>();
    private readonly IOptions<IdentityOptions> _identityOptions;

    public SignInViaPasswordHandler(
        UserManager<AuthUser> userManager,
        SignInManager<AuthUser> signInManager,
        IAuthenticationFlowHandler authFlowHandler,
        IOptions<IdentityOptions> identityOptions)
        : base(signInManager, authFlowHandler)
    {
        EnsureArg.IsNotNull(userManager, nameof(userManager));

        _userManager = userManager;
        _identityOptions = identityOptions;
    }

    protected override Task<AuthUser?> FindAuthUserAsync(
        SignInViaPassword command,
        CancellationToken cancellationToken)
        => _userManager.FindByNameAsync(command.UserName)!;

    protected override async Task<Domain.SignInResult> SignInAsync(
        SignInViaPassword command,
        AuthUser authUser,
        CancellationToken cancellationToken)
    {
        var status = SignInStatus.Success;
        var statusMessage = string.Empty;

        var signInResult = await SignInManager.PasswordSignInAsync(
            command.UserName,
            command.Password,
            command.RememberMe,
            true).ConfigureAwait(false); //Note! We always lockout on failure

        if (!signInResult.Succeeded)
        {
            status = SignInStatus.Failure;
            statusMessage = "Incorrect username or password.";

            if (signInResult.RequiresTwoFactor)
            {
                status = SignInStatus.RequiresVerification;
                statusMessage = "User account requires verification.";
                Logger.LogInformation("{@UserName} requires verification", command.UserName);
            }
            else if (signInResult.IsLockedOut)
            {
                status = SignInStatus.LockedOut;
                Logger.LogInformation("{@UserName} locked out", command.UserName);
            }
            else
            {
                var remainingAttempts = _userManager.Options.Lockout.MaxFailedAccessAttempts - authUser.AccessFailedCount;
                Logger.LogInformation("{@UserName} attempted to login with an incorrect password. " +
                    "Remaining attempt(s) before lock out: {@RemainingAttempts}", command.UserName, remainingAttempts);
            }
        }
        else
        {
            var isTwoFactorAuth = await AuthFlowHandler.IsTwoFactorAthenticationRequiredForLoginAsync(authUser.Id, cancellationToken).ConfigureAwait(false);

            if (isTwoFactorAuth && !authUser.TwoFactorEnabled)
            {
                status = SignInStatus.ConfigureTFA;
                statusMessage = "Two Factor Authentication must be configured.";
                Logger.LogInformation("{@UserName} must configure two factor authentication", command.UserName);
            }
        }

        return new Domain.SignInResult(status, (int)_identityOptions.Value.Lockout.DefaultLockoutTimeSpan.TotalMinutes);
    }
}
