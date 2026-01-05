using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Identity;
using OneBeyond.Studio.Core.Mediator.Queries;
using OneBeyond.Studio.Obelisk.Authentication.Application.Entities;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Queries;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.QueryHandlers;

internal sealed class GetLoginForProviderLoginHandler
    : IQueryHandler<GetLoginForProviderLogin, string?>
{
    private readonly UserManager<AuthUser> _userManager;

    public GetLoginForProviderLoginHandler(
        UserManager<AuthUser> userManager)
    {
        EnsureArg.IsNotNull(userManager, nameof(userManager));

        _userManager = userManager;
    }

    public async Task<string?> HandleAsync(
        GetLoginForProviderLogin query,
        CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(query, nameof(query));

        var authUser = await _userManager.FindByLoginAsync(
                query.Login.SignInProviderId,
                query.Login.ProviderLoginId)
            .ConfigureAwait(false);

        return authUser?.Id;
    }
}
