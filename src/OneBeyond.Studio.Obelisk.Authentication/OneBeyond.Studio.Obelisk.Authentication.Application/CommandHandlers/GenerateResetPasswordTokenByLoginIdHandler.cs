using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OneBeyond.Studio.Obelisk.Authentication.Application.Entities;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Exceptions;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.CommandHandlers;

internal sealed class GenerateResetPasswordTokenByLoginIdHandler : IRequestHandler<GenerateResetPasswordTokenByLoginId, string>
{
    private readonly UserManager<AuthUser> _userManager;

    public GenerateResetPasswordTokenByLoginIdHandler(
        UserManager<AuthUser> userManager
        )
    {
        EnsureArg.IsNotNull(userManager, nameof(userManager));

        _userManager = userManager;
    }

    public async Task<string> Handle(GenerateResetPasswordTokenByLoginId command, CancellationToken cancellationToken)
    {
        var identityUser = await _userManager.FindByIdAsync(command.LoginId).ConfigureAwait(false)
            ?? throw new AuthLoginNotFoundException($"Login with id {command.LoginId} not found");

        return await _userManager.GeneratePasswordResetTokenAsync(identityUser).ConfigureAwait(false);
    }
}
