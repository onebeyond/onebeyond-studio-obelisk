using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OneBeyond.Studio.Obelisk.Authentication.Application.Entities;
using OneBeyond.Studio.Obelisk.Authentication.Domain;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Exceptions;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.CommandHandlers;

internal sealed class GenerateResetPasswordTokenByEmailHandler : IRequestHandler<GenerateResetPasswordTokenByEmail, ResetPasswordToken>
{
    private readonly UserManager<AuthUser> _userManager;

    public GenerateResetPasswordTokenByEmailHandler(
        UserManager<AuthUser> userManager
        )
    {
        EnsureArg.IsNotNull(userManager, nameof(userManager));

        _userManager = userManager;
    }

    public async Task<ResetPasswordToken> Handle(GenerateResetPasswordTokenByEmail command, CancellationToken cancellationToken)
    {
        var identityUser = await _userManager.FindByEmailAsync(command.Email).ConfigureAwait(false)
            ?? throw new AuthLoginNotFoundException($"Login with email {command.Email} not found");

        var resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(identityUser).ConfigureAwait(false);

        return new ResetPasswordToken(identityUser.Id, resetPasswordToken);
    }
}
