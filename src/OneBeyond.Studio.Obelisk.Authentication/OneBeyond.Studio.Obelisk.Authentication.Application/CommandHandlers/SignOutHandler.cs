using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OneBeyond.Studio.Obelisk.Authentication.Application.Entities;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.CommandHandlers;

internal sealed class SignOutHandler : IRequestHandler<SignOut>
{
    private readonly SignInManager<AuthUser> _signInManager;
    private readonly UserManager<AuthUser> _userManager;

    public SignOutHandler(
        UserManager<AuthUser> userManager,
        SignInManager<AuthUser> signInManager
        )
    {
        EnsureArg.IsNotNull(userManager, nameof(userManager));
        EnsureArg.IsNotNull(signInManager, nameof(signInManager));

        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task Handle(SignOut command, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        var identityUser = await _userManager.FindByIdAsync(command.LoginId).ConfigureAwait(false);
        //We use SecurityStampValidator to invalidate all existing auth cookies for this user
        await _userManager.UpdateSecurityStampAsync(identityUser!).ConfigureAwait(false); 
        await _signInManager.SignOutAsync().ConfigureAwait(false);
    }
}
