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

internal sealed class UpdateLoginHandler : IRequestHandler<UpdateLogin>
{
    private readonly UserManager<AuthUser> _userManager;

    public UpdateLoginHandler(
        UserManager<AuthUser> userManager
        )
    {
        EnsureArg.IsNotNull(userManager, nameof(userManager));
        _userManager = userManager;
    }

    public async Task Handle(UpdateLogin command, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        var identityUser = await _userManager.FindByIdAsync(command.LoginId).ConfigureAwait(false)
            ?? throw new AuthLoginNotFoundException($"Login with id {command.LoginId} not found");

        identityUser.UserName = command.UserName;
        identityUser.Email = command.Email;

        var existingRole = (await _userManager.GetRolesAsync(identityUser).ConfigureAwait(false)).FirstOrDefault();

        if (existingRole != null && command.RoleId != existingRole)
        {
            await _userManager.RemoveFromRoleAsync(identityUser, existingRole).ConfigureAwait(false);
        }

        if (command.RoleId != null && command.RoleId != existingRole)
        {
            await _userManager.AddToRoleAsync(identityUser, command.RoleId).ConfigureAwait(false);
        }

        await _userManager.UpdateAsync(identityUser).ConfigureAwait(false);
    }

}
