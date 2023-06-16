using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Identity;
using OneBeyond.Studio.Obelisk.Authentication.Application.Entities;
using OneBeyond.Studio.Obelisk.Authentication.Domain;
using OneBeyond.Studio.Obelisk.Authentication.Domain.AuthenticationFlows;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.CommandHandlers;

internal sealed class SignInViaProviderHandler : SignInHandler<SignInViaProvider>
{
    private readonly UserManager<AuthUser> _userManager;

    public SignInViaProviderHandler(
        UserManager<AuthUser> userManager,
        SignInManager<AuthUser> signInManager,
        IAuthenticationFlowHandler authFlowHandler)
        : base(signInManager, authFlowHandler)
    {
        EnsureArg.IsNotNull(userManager, nameof(userManager));

        _userManager = userManager;
    }

    protected override Task<AuthUser?> FindAuthUserAsync(
        SignInViaProvider command,
        CancellationToken cancellationToken)
        => _userManager.FindByLoginAsync(
            command.Login.SignInProviderId,
            command.Login.ProviderLoginId)!;

    protected override async Task<Domain.SignInResult> SignInAsync(
        SignInViaProvider command,
        AuthUser authUser,
        CancellationToken cancellationToken)
    {
        var status = SignInStatus.Success;
        var statusMessage = string.Empty;

        var signInResult = await SignInManager.ExternalLoginSignInAsync(
                command.Login.SignInProviderId,
                command.Login.ProviderLoginId,
                isPersistent: false,
                bypassTwoFactor: true) //Note: we want do not enable 2FA with external login by default, if this needs to be changed for your project, please do
            .ConfigureAwait(false);

        if (!signInResult.Succeeded)
        {
            status = SignInStatus.Failure;
            statusMessage = $"Sign in with provider {command.Login.SignInProviderId} failed.";
        }

        return new Domain.SignInResult(status);
    }
}
