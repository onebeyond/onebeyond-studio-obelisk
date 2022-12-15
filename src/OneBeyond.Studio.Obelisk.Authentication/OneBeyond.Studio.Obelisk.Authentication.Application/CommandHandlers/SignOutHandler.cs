using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OneBeyond.Studio.Obelisk.Authentication.Application.Entities;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.CommandHandlers;

internal sealed class SignOutHandler : IRequestHandler<SignOut, Unit>
{
    private readonly SignInManager<AuthUser> _signInManager;

    public SignOutHandler(
        SignInManager<AuthUser> signInManager
        )
    {
        EnsureArg.IsNotNull(signInManager, nameof(signInManager));

        _signInManager = signInManager;
    }

    public async Task<Unit> Handle(SignOut command, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        await _signInManager.SignOutAsync().ConfigureAwait(false);

        return Unit.Value;
    }
}
