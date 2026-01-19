using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Identity;
using OneBeyond.Studio.Core.Mediator;
using OneBeyond.Studio.Obelisk.Authentication.Application.Entities;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Exceptions;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Queries;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.QueryHandlers;

internal sealed class GetExternalLoginInformationHandler
    : IRequestHandler<GetExternalLoginInformation, Domain.ExternalLoginInfo>
{
    private readonly SignInManager<AuthUser> _signInManager;

    public GetExternalLoginInformationHandler(SignInManager<AuthUser> signInManager)
    {
        EnsureArg.IsNotNull(signInManager, nameof(signInManager));

        _signInManager = signInManager;
    }

    public async Task<Domain.ExternalLoginInfo> Handle(
        GetExternalLoginInformation query,
        CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(query, nameof(query));

        var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync()
            .ConfigureAwait(false);

        return externalLoginInfo == null
            ? throw new AuthException("Error retrieving external login information")
            : new Domain.ExternalLoginInfo(
                externalLoginInfo.Principal,
                externalLoginInfo.LoginProvider,
                externalLoginInfo.ProviderKey,
                externalLoginInfo.ProviderDisplayName ?? "N/A");
    }
}
