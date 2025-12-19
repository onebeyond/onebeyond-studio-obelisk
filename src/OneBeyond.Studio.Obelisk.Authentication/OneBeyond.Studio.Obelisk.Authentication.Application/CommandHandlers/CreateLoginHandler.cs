using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Identity;
using OneBeyond.Studio.Core.Mediator.Commands;
using OneBeyond.Studio.Obelisk.Authentication.Application.Entities;
using OneBeyond.Studio.Obelisk.Authentication.Domain;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Exceptions;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.CommandHandlers;

internal sealed class CreateLoginHandler : ICommandHandler<CreateLogin, ResetPasswordToken>
{
    private readonly UserManager<AuthUser> _userManager;

    public CreateLoginHandler(UserManager<AuthUser> userManager)
    {
        EnsureArg.IsNotNull(userManager, nameof(userManager));

        _userManager = userManager;
    }

    public async Task<ResetPasswordToken> HandleAsync(CreateLogin command, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        var identityUser = new AuthUser
        {
            UserName = command.UserName,
            Email = command.Email,
            Id = Guid.NewGuid().ToString(),
            PasswordResetKey = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace("/", "-").Replace("+", "_").Replace("=", "")
        };

        var res = await _userManager.CreateAsync(identityUser).ConfigureAwait(false);

        if (!res.Succeeded)
        {
            var errorMessage = string.Join(
                ",",
                res.Errors.Select(x => x.Description));

            throw new AuthException(errorMessage);
        }

        var newUser = await _userManager.FindByIdAsync(identityUser.Id).ConfigureAwait(false)
            ?? throw new AuthLoginNotFoundException();

        if (!string.IsNullOrWhiteSpace(command.RoleId))
        {
            await _userManager.AddToRoleAsync(newUser, command.RoleId).ConfigureAwait(false);
        }

        if (command.ExternalLogin != null)
        {
            await _userManager.AddLoginAsync(
                newUser,
                new UserLoginInfo(
                command.ExternalLogin.Value.SignInProviderId,
                command.ExternalLogin.Value.ProviderLoginId,
                command.ExternalLogin.Value.SignInProviderDisplayName));
        }

        var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(newUser).ConfigureAwait(false);

        return new ResetPasswordToken(newUser.Id, passwordResetToken);
    }

}
