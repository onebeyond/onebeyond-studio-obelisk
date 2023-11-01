using System;
using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OneBeyond.Studio.Application.SharedKernel.AmbientContexts;
using OneBeyond.Studio.Crosscuts.Logging;
using OneBeyond.Studio.Obelisk.Application.Features.Users.Dto;
using OneBeyond.Studio.Obelisk.Application.Features.Users.Queries;
using OneBeyond.Studio.Obelisk.Application.Services.AmbientContexts;
using OneBeyond.Studio.Obelisk.Authentication.Domain;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Commands;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Commands;
using OneBeyond.Studio.Obelisk.WebApi.Helpers;
using OneBeyond.Studio.Obelisk.WebApi.Requests.Auth;
using AmbientContext = OneBeyond.Studio.Obelisk.Application.Services.AmbientContexts.AmbientContext;
using SignInResult = OneBeyond.Studio.Obelisk.Authentication.Domain.SignInResult;

namespace OneBeyond.Studio.Obelisk.WebApi.Controllers;

[Produces("application/json")]
[ApiVersion("1.0")]
public sealed class AuthController : ControllerBase
{
    private static readonly ILogger Logger = LogManager.CreateLogger<AuthController>();

    private readonly IMediator _mediator;
    private readonly ClientApplicationLinkGenerator _clientApplicationLinkGenerator;
    private readonly IOptions<IdentityOptions> _identityOptions;
    private readonly AmbientContext _ambientContext;

    public AuthController(
        IMediator mediator,
        IAmbientContextAccessor<AmbientContext> ambientContextAccessor,
        ClientApplicationLinkGenerator clientApplicationLinkGenerator,
        IOptions<IdentityOptions> identityOptions)
    {
        EnsureArg.IsNotNull(mediator, nameof(mediator));
        EnsureArg.IsNotNull(ambientContextAccessor, nameof(ambientContextAccessor));
        EnsureArg.IsNotNull(clientApplicationLinkGenerator, nameof(clientApplicationLinkGenerator));
        EnsureArg.IsNotNull(identityOptions, nameof(identityOptions));

        _mediator = mediator;
        _ambientContext = ambientContextAccessor.AmbientContext;
        _clientApplicationLinkGenerator = clientApplicationLinkGenerator;
        _identityOptions = identityOptions;
    }

    [Authorize]
    [HttpGet("WhoAmI")]
    public Task<WhoAmIDto> WhoAmI(CancellationToken cancellationToken)
        => _mediator.Send(new WhoAmI(), cancellationToken);

    [HttpPost("Basic/SignIn")]
    public Task<SignInResult> BasicSignIn(
        [FromBody] SignInViaPassword signInViaPassword,
        CancellationToken cancellationToken)
        => _mediator.Send(signInViaPassword, cancellationToken);

    [HttpPost("Basic/SignInWithTwoFA")]
    public Task<SignInResult> SignInWithTwoFA(
        [FromBody] SignInTfa signInViaTfa,
        CancellationToken cancellationToken)
        => _mediator.Send(signInViaTfa, cancellationToken);

    [HttpPost("Basic/SignInWithRecoveryCode")]
    public Task<SignInWithRecoveryCodeResult> SignInWithRecoveryCode(
    [FromBody] SignInTfaWithRecoveryCode signInViaRecoveryCode,
    CancellationToken cancellationToken)
    => _mediator.Send(signInViaRecoveryCode, cancellationToken);

    [Authorize]
    [HttpPost("SignOut")]
    public Task SignOut(CancellationToken cancellationToken)
        => _mediator.Send(new SignOut(_ambientContext.GetUserContext().UserAuthId), cancellationToken);


    [HttpPost("ForgotPassword")]
    public async Task ForgotPassword(
        [FromBody] ForgotPasswordRequest forgotPassword,
        CancellationToken cancellationToken)
    {
        // To guard against timing attacks
        Thread.Sleep(new Random().Next(1000, 3000));

        try
        {
            var resetPasswordTokenResult = await _mediator.Send(
                new GenerateResetPasswordTokenByEmail(forgotPassword.Email), cancellationToken)
            .ConfigureAwait(false);

            await _mediator.Send(
                new SendResetPasswordEmail(
                        resetPasswordTokenResult.LoginId,
                        _clientApplicationLinkGenerator.GetResetPasswordUrl(resetPasswordTokenResult.LoginId, resetPasswordTokenResult.Value)),
                    cancellationToken).ConfigureAwait(false);
        }
        catch (Exception)
        {
            // Don't throw exception so we don't reveal which emails are currently in use
            Logger.LogInformation("Error in forgot password");
        }
    }

    [HttpPost("ResetPassword")]
    public Task<ResetPasswordStatus> ResetPassword(
        [FromBody] ResetPasswordRequest resetPassword,
        CancellationToken cancellationToken)
        => _mediator.Send(new ResetPassword(
            resetPassword.UserId,
            resetPassword.Token,
            resetPassword.Password),
            cancellationToken);

    [Authorize]
    [HttpPost("ChangePassword")]
    public Task<ChangePasswordResult> ChangePassword(
        [FromBody] ChangePasswordRequest changePassword,
        CancellationToken cancellationToken)
        => _mediator.Send(new ChangePassword(
            _ambientContext.GetUserContext().UserAuthId,
            changePassword.OldPassword,
            changePassword.NewPassword), cancellationToken);

    [HttpGet("PasswordRequirements")]
    public PasswordOptions PasswordRequirements()
        => _identityOptions.Value.Password;

    /// <summary>
    /// Empty action used for keeping session alive when user pressed cancel logout.
    /// </summary>
    [HttpGet("Ping")]
    public IActionResult Ping()
        => Ok();
}
