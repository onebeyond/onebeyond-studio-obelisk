using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OneBeyond.Studio.Obelisk.Authentication.Application.Entities;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Exceptions;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.CommandHandlers;

internal sealed class ResetPasswordHandler : IRequestHandler<ResetPassword>
{
    private readonly UserManager<AuthUser> _userManager;

    public ResetPasswordHandler(
        UserManager<AuthUser> userManager
        )
    {
        EnsureArg.IsNotNull(userManager, nameof(userManager));
        _userManager = userManager;
    }

    public async Task Handle(ResetPassword command, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        ValidateInput(command);

        var identityUser = await FindUserByIdAsync(command.UserId);

        await VerifyTokenAsync(identityUser, command.Token);

        await ResetUserPasswordAsync(identityUser, command.Token, command.Password);
    }

    private void ValidateInput(ResetPassword command)
    {
        if (command.Token is null)
        {
            throw new ArgumentException("Invalid token.");
        }

        if (command.UserId is null)
        {
            throw new ArgumentException("Invalid user ID.");
        }
    }

    private async Task<AuthUser> FindUserByIdAsync(string userId)
    {
        var identityUser = await _userManager.FindByIdAsync(userId).ConfigureAwait(false);

        return identityUser is null ? throw new AuthLoginNotFoundException($"Login with the {userId} is not found.") : identityUser;
    }

    private async Task VerifyTokenAsync(AuthUser identityUser, string token)
    {
        var isTokenValid = await _userManager.VerifyUserTokenAsync(
            identityUser,
            _userManager.Options.Tokens.PasswordResetTokenProvider,
            "ResetPassword",
            token);

        if (!isTokenValid)
        {
            throw new AuthLoginNotFoundException("Invalid token.");
        }
    }

    private async Task ResetUserPasswordAsync(AuthUser identityUser, string token, string newPassword)
    {
        var resetPasswordResult = await _userManager.ResetPasswordAsync(identityUser, token, newPassword).ConfigureAwait(false);

        if (!resetPasswordResult.Succeeded)
        {
            throw new AuthException(string.Join(",", resetPasswordResult.Errors.Select(x => x.Description)));
        }
    }

}
