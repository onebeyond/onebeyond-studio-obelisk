using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OneBeyond.Studio.Obelisk.Authentication.Application.Entities;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Exceptions;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.CommandHandlers;

internal sealed class UnlockLoginHandler : IRequestHandler<UnlockLogin>
{
    private readonly UserManager<AuthUser> _userManager;

    public UnlockLoginHandler(
        UserManager<AuthUser> userManager
        )
    {
        EnsureArg.IsNotNull(userManager, nameof(userManager));
        _userManager = userManager;
    }

    public async Task Handle(UnlockLogin command, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        var identityUser = await _userManager.FindByIdAsync(command.LoginId).ConfigureAwait(false)
            ?? throw new AuthLoginNotFoundException($"Login with id {command.LoginId} not found");

        identityUser.LockoutEnd = null;

        await _userManager.UpdateAsync(identityUser).ConfigureAwait(false);
    }

}
