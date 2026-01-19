using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using OneBeyond.Studio.Core.Mediator;
using OneBeyond.Studio.Crosscuts.Logging;
using OneBeyond.Studio.Obelisk.Authentication.Application.Entities;
using OneBeyond.Studio.Obelisk.Authentication.Application.Services.JwtAuthentication;
using OneBeyond.Studio.Obelisk.Authentication.Domain.JwtAuthentication;
using OneBeyond.Studio.Obelisk.Authentication.Domain.JwtAuthentication.Commands;
using OneBeyond.Studio.Obelisk.Authentication.Domain.JwtAuthentication.Exceptions;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.JwtAuthentication.CommandHandlers;

internal sealed class SignInJwtTokenHandler : IRequestHandler<SignInJwtToken, JwtToken>
{
    private static readonly ILogger Logger = LogManager.CreateLogger<SignInJwtTokenHandler>();

    private readonly UserManager<AuthUser> _userManager;
    private readonly SignInManager<AuthUser> _signInManager;
    private readonly IJwtTokenService _jWTokenService;

    public SignInJwtTokenHandler(
        UserManager<AuthUser> userManager,
        SignInManager<AuthUser> signInManager,
        IJwtTokenService jWTokenservice)
    {
        EnsureArg.IsNotNull(signInManager, nameof(signInManager));
        EnsureArg.IsNotNull(userManager, nameof(userManager));
        EnsureArg.IsNotNull(jWTokenservice, nameof(jWTokenservice));

        _userManager = userManager;
        _signInManager = signInManager;
        _jWTokenService = jWTokenservice;
    }

    public async Task<JwtToken> Handle(SignInJwtToken command, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        var identityUser = await _userManager.FindByNameAsync(command.Username).ConfigureAwait(false)
            ?? throw new JwtAuthenticationFailedException($"Login with username {command.Username} not found");

        // Check that the user can sign in and is not locked out.
        // If two-factor authentication is supported, it would also be appropriate to check that 2FA is enabled for the user
        // And also check the sign in result for RequiresTwoFactor error
        if (!await _signInManager.CanSignInAsync(identityUser).ConfigureAwait(false)
            || _userManager.SupportsUserLockout && await _userManager.IsLockedOutAsync(identityUser).ConfigureAwait(false))
        {
            throw new JwtAuthenticationFailedException("User account is locked out.");
        }

        var signInResult = await _signInManager.CheckPasswordSignInAsync(
                identityUser,
                command.Password,
                _userManager.SupportsUserLockout)
            .ConfigureAwait(false);

        if (!signInResult.Succeeded)
        {
            var statusMessage = "Incorrect username or password.";

            if (signInResult.IsLockedOut)
            {
                Logger.LogInformation("{UserName} is locked out", identityUser.UserName);
            }
            else if (_userManager.SupportsUserLockout)
            {
                var remainingAttempts = _userManager.Options.Lockout.MaxFailedAccessAttempts - identityUser.AccessFailedCount;

                Logger.LogInformation(
                    "{UserName} attempted to login with an incorrect password. " +
                    "Remaining attempt(s) before lock out: {RemainingAttempts}",
                    identityUser.UserName,
                    remainingAttempts);
            }

            throw new JwtAuthenticationFailedException(statusMessage);
        }

        if (_userManager.SupportsUserLockout)
        {
            await _userManager.ResetAccessFailedCountAsync(identityUser);
        }

        return await _jWTokenService.CreateTokenAsync(
                identityUser,
                cancellationToken)
            .ConfigureAwait(false);
    }
}
