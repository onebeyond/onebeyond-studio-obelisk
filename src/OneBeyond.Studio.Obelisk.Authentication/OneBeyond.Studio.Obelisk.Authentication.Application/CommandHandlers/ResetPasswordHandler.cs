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

internal sealed class ResetPasswordHandler : IRequestHandler<ResetPassword, Unit>
{
    private readonly UserManager<AuthUser> _userManager;

    public ResetPasswordHandler(
        UserManager<AuthUser> userManager
        )
    {
        EnsureArg.IsNotNull(userManager, nameof(userManager));
        _userManager = userManager;
    }

    public async Task<Unit> Handle(ResetPassword command, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        var identityUser = await _userManager.FindByNameAsync(command.UserName).ConfigureAwait(false)
            ?? throw new AuthLoginNotFoundException($"Login with username {command.UserName} not found");

        var resetPasswordResult = await _userManager.ResetPasswordAsync(identityUser, command.ResetPasswordCode, command.Password).ConfigureAwait(false);

        return !resetPasswordResult.Succeeded
            ? throw new AuthException(string.Join(",", resetPasswordResult.Errors.Select(x => x.Description)))
            : Unit.Value;
    }
}
