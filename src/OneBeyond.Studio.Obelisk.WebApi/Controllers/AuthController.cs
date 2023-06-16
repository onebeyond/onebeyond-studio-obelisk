using System;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OneBeyond.Studio.Crosscuts.Utilities.Identities;
using OneBeyond.Studio.Obelisk.Application.Exceptions;
using OneBeyond.Studio.Obelisk.Application.Features.Users.Dto;
using OneBeyond.Studio.Obelisk.Application.Features.Users.Queries;
using OneBeyond.Studio.Obelisk.Authentication.Domain;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Commands;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Commands;
using OneBeyond.Studio.Obelisk.WebApi.Helpers;
using OneBeyond.Studio.Obelisk.WebApi.Requests.Auth;
using SignInResult = OneBeyond.Studio.Obelisk.Authentication.Domain.SignInResult;

namespace OneBeyond.Studio.Obelisk.WebApi.Controllers;

[Produces("application/json")]
[ApiVersion("1.0")]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ClientApplicationLinkGenerator _clientApplicationLinkGenerator;
    private readonly IOptions<IdentityOptions> _identityOptions;

    public AuthController(
        IMediator mediator,
        ClientApplicationLinkGenerator clientApplicationLinkGenerator,
        IOptions<IdentityOptions> identityOptions)
    {
        EnsureArg.IsNotNull(mediator, nameof(mediator));
        EnsureArg.IsNotNull(clientApplicationLinkGenerator, nameof(clientApplicationLinkGenerator));
        EnsureArg.IsNotNull(identityOptions, nameof(identityOptions));

        _mediator = mediator;
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
        => _mediator.Send(new SignOut(), cancellationToken);


    [HttpPost("ForgotPassword")]
    public async Task ForgotPassword(
        [FromBody] ForgotPasswordRequest forgotPassword, 
        CancellationToken cancellationToken)
    {
        try
        {
            var resetPasswordTokenResult = await _mediator.Send(
                new GenerateResetPasswordTokenByEmail(forgotPassword.Email), cancellationToken)
            .ConfigureAwait(false);

            await _mediator.Send(
                new SendResetPasswordEmail(
                        resetPasswordTokenResult.LoginId,
                        _clientApplicationLinkGenerator.GetResetPasswordUrl(resetPasswordTokenResult.LoginId,resetPasswordTokenResult.Value)),
                    cancellationToken).ConfigureAwait(false);
        }
        catch (Exception)
        {
            throw new ObeliskApplicationException("Failed to reset user password");
        }
    }

    [HttpPost("ResetPassword")]
    public async Task ResetPassword(
        [FromBody] ResetPasswordRequest resetPassword, 
        CancellationToken cancellationToken)
    {
        try
        {
            await _mediator.Send(new ResetPassword(
                   resetPassword.UserId,
                   resetPassword.Token,
                   resetPassword.Password),
               cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw new ObeliskApplicationException(ex.Message);
        }
    }

    [Authorize]
    [HttpPost("ChangePassword")]
    public Task<ChangePasswordResult> ChangePassword(
        [FromBody] ChangePasswordRequest changePassword, 
        CancellationToken cancellationToken)
        => _mediator.Send(new ChangePassword(
            HttpContext.User?.Identity?.TryGetLoginId() ?? throw new ObeliskApplicationException("Failed to retrieve the ID of a logged in user"),
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
