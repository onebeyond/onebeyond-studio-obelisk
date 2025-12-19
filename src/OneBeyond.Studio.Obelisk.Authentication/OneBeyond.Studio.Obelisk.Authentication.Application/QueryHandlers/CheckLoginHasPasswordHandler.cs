using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Identity;
using OneBeyond.Studio.Core.Mediator.Queries;
using OneBeyond.Studio.Obelisk.Authentication.Application.Entities;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Exceptions;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Queries;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.QueryHandlers;

internal sealed class CheckLoginHasPasswordHandler : IQueryHandler<CheckLoginHasPassword, bool>
{
    private readonly UserManager<AuthUser> _userManager;

    public CheckLoginHasPasswordHandler(
        UserManager<AuthUser> userManager
    )
    {
        EnsureArg.IsNotNull(userManager, nameof(userManager));

        _userManager = userManager;
    }

    public async Task<bool> HandleAsync(CheckLoginHasPassword query, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(query, nameof(query));

        var identityUser = await _userManager.FindByIdAsync(query.LoginId).ConfigureAwait(false)
            ?? throw new AuthLoginNotFoundException($"Login with Id {query.LoginId} not found");

        return await _userManager.HasPasswordAsync(identityUser).ConfigureAwait(false);
    }
}
