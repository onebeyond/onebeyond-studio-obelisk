using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OneBeyond.Studio.Obelisk.Authentication.Application.Entities;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Exceptions;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Commands;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.TfaAuthentication.CommandHandlers;

internal sealed class ForgetTfaClientHandler : IRequestHandler<ForgetTfaClient, Unit>
{
    private readonly SignInManager<AuthUser> _signInManager;
    private readonly UserManager<AuthUser> _userManager;

    public ForgetTfaClientHandler(
        UserManager<AuthUser> userManager,
        SignInManager<AuthUser> signInManager
        )
    {
        EnsureArg.IsNotNull(userManager, nameof(userManager));
        EnsureArg.IsNotNull(signInManager, nameof(signInManager));

        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<Unit> Handle(ForgetTfaClient command, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        _ = await _userManager.FindByIdAsync(command.LoginId).ConfigureAwait(false)
            ?? throw new AuthLoginNotFoundException($"Login with id {command.LoginId} not found");

        await _signInManager.ForgetTwoFactorClientAsync().ConfigureAwait(false);

        return Unit.Value;
    }
}
