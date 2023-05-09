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

internal sealed class SetPasswordHandler : IRequestHandler<SetPassword>
{
    private readonly UserManager<AuthUser> _userManager;
    private readonly SignInManager<AuthUser> _signInManager;

    public SetPasswordHandler(
        UserManager<AuthUser> userManager,
        SignInManager<AuthUser> signInManager
        )
    {
        EnsureArg.IsNotNull(userManager, nameof(userManager));
        EnsureArg.IsNotNull(signInManager, nameof(signInManager));

        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task Handle(SetPassword command, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        var identityUser = await _userManager.FindByIdAsync(command.LoginId).ConfigureAwait(false)
            ?? throw new AuthLoginNotFoundException($"Login with id {command.LoginId} not found");

        var hasPassword = await _userManager.HasPasswordAsync(identityUser).ConfigureAwait(false);

        if (hasPassword)
        {
            throw new AuthException("Login already has a password set, use ChangePassword to change it");
        }

        var addPasswordResult = await _userManager.ResetPasswordAsync(identityUser, command.ResetPasswordCode, command.Password).ConfigureAwait(false);

        if (!addPasswordResult.Succeeded)
        {
            throw new AuthException(string.Join(",", addPasswordResult.Errors.Select(x => x.Description)));
        }

        await _signInManager.SignInAsync(identityUser, isPersistent: false).ConfigureAwait(false);
    }
}
