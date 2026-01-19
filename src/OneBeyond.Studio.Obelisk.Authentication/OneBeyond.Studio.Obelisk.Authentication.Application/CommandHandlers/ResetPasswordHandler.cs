using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using OneBeyond.Studio.Core.Mediator;
using OneBeyond.Studio.Crosscuts.Logging;
using OneBeyond.Studio.Obelisk.Authentication.Application.Entities;
using OneBeyond.Studio.Obelisk.Authentication.Domain;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Exceptions;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.CommandHandlers;

internal sealed class ResetPasswordHandler : IRequestHandler<ResetPassword, ResetPasswordStatus>
{
    private readonly UserManager<AuthUser> _userManager;
    private static readonly ILogger Logger = LogManager.CreateLogger<ResetPasswordHandler>();

    public ResetPasswordHandler(
        UserManager<AuthUser> userManager
        )
    {
        EnsureArg.IsNotNull(userManager, nameof(userManager));
        _userManager = userManager;
    }

    public async Task<ResetPasswordStatus> Handle(ResetPassword command, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        try
        {
            var identityUser = await FindUserByIdAsync(command.UserId);
            return await ResetUserPasswordAsync(identityUser, command.Token, command.Password);
        }
        catch (AuthLoginNotFoundException ex)
        {
            Logger.LogInformation(ex.Message);
            return ResetPasswordStatus.InvalidToken;
        }
        catch (Exception ex)
        {
            Logger.LogInformation("{@UserId} reset password exception: {@exception}", command.UserId, ex.Message);
            return ResetPasswordStatus.OtherError;
        }
    }

    private async Task<AuthUser> FindUserByIdAsync(string userId)
    {
        var identityUser = await _userManager.FindByIdAsync(userId).ConfigureAwait(false);

        return identityUser is null ? throw new AuthLoginNotFoundException($"Login with the {userId} is not found.") : identityUser;
    }

    private async Task<ResetPasswordStatus> ResetUserPasswordAsync(AuthUser identityUser, string token, string newPassword)
    {
        var resetPasswordResult = await _userManager.ResetPasswordAsync(identityUser, token, newPassword).ConfigureAwait(false);
        var resetPasswordStatus = ResetPasswordStatus.Success;
        if (!resetPasswordResult.Succeeded)
        {
            var firstErrorCode = resetPasswordResult.Errors.FirstOrDefault()?.Code;
            if (firstErrorCode == null)
            {
                resetPasswordStatus = ResetPasswordStatus.OtherError;
            }
            else if (firstErrorCode == ResetPasswordStatus.InvalidToken.ToString())
            {
                resetPasswordStatus = ResetPasswordStatus.InvalidToken;
            }
        }
        return resetPasswordStatus;
    }

}
