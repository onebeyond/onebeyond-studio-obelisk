using System;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using OneBeyond.Studio.Core.Mediator;
using OneBeyond.Studio.Crosscuts.Exceptions;
using OneBeyond.Studio.Crosscuts.Logging;
using OneBeyond.Studio.Obelisk.Authentication.Application.Entities;
using OneBeyond.Studio.Obelisk.Authentication.Domain;
using OneBeyond.Studio.Obelisk.Authentication.Domain.AuthenticationFlows;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.CommandHandlers;

internal abstract class SignInHandler<TSignIn> : IRequestHandler<TSignIn, Domain.SignInResult>
    where TSignIn : SignIn
{
    private static readonly ILogger Logger = LogManager.CreateLogger<TSignIn>();

    protected SignInHandler(
        SignInManager<AuthUser> signInManager,
        IAuthenticationFlowHandler authFlowHandler)
    {
        EnsureArg.IsNotNull(signInManager, nameof(signInManager));
        EnsureArg.IsNotNull(authFlowHandler, nameof(authFlowHandler));

        AuthFlowHandler = authFlowHandler;
        SignInManager = signInManager;
    }

    protected IAuthenticationFlowHandler AuthFlowHandler { get; }
    protected SignInManager<AuthUser> SignInManager { get; }

    public async Task<Domain.SignInResult> Handle(
        TSignIn command,
        CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        var authUser = await FindAuthUserAsync(command, cancellationToken).ConfigureAwait(false);

        if (authUser is null)
        {
            Logger.LogInformation("User attempted to login with unrecognised account");
            return new Domain.SignInResult(
                SignInStatus.Failure);
        }

        try
        {
            await AuthFlowHandler.OnValidatingLoginAsync(
                authUser.Id,
                cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        when (!exception.IsCritical())
        {
            return new Domain.SignInResult(SignInStatus.Failure);
        }

        var signInResult = await SignInAsync(command, authUser, cancellationToken).ConfigureAwait(false);

        try
        {
            await AuthFlowHandler.OnSignInCompletedAsync(
                authUser.Id,
                signInResult.Status,
                cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        when (!exception.IsCritical())
        {
            await SignInManager.SignOutAsync().ConfigureAwait(false);
            return new Domain.SignInResult(SignInStatus.Failure);
        }

        return signInResult;
    }

    protected abstract Task<AuthUser?> FindAuthUserAsync(
        TSignIn command,
        CancellationToken cancellationToken);

    protected abstract Task<Domain.SignInResult> SignInAsync(
        TSignIn command,
        AuthUser authUser,
        CancellationToken cancellationToken);
}
