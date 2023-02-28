using System;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Commands;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Commands;
using OneBeyond.Studio.Obelisk.WebApi.Helpers;
using SignInResult = OneBeyond.Studio.Obelisk.Authentication.Domain.SignInResult;

namespace OneBeyond.Studio.Obelisk.WebApi.Controllers;

[Produces("application/json")]
[ApiVersion("1.0")]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ClientApplicationLinkGenerator _clientApplicationLinkGenerator;

    public AuthController(
        IMediator mediator,
        ClientApplicationLinkGenerator clientApplicationLinkGenerator)
    {
        EnsureArg.IsNotNull(mediator, nameof(mediator));
        EnsureArg.IsNotNull(clientApplicationLinkGenerator, nameof(clientApplicationLinkGenerator));

        _mediator = mediator;
        _clientApplicationLinkGenerator = clientApplicationLinkGenerator;
    }

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
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPassword, CancellationToken cancellationToken)
    {
        try
        {
            var resetPasswordTokenResult = await _mediator.Send(
                new GenerateResetPasswordTokenByEmail(forgotPassword.Email!), cancellationToken)
            .ConfigureAwait(false);

            await _mediator.Send(
                new SendResetPasswordEmail(
                        resetPasswordTokenResult.LoginId,
                        _clientApplicationLinkGenerator.GetResetPasswordUrl(resetPasswordTokenResult.Value)),
                    cancellationToken).ConfigureAwait(false);

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest();
        }
    }

    [HttpPost("ResetPassword")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPassword, CancellationToken cancellationToken)
    {
        try
        {
            await _mediator.Send(new ResetPassword(
                   resetPassword.UserName!,
                   resetPassword.Code!,
                   resetPassword.Password!),
               cancellationToken).ConfigureAwait(false);

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest();
        }
    }

    /// <summary>
    /// Empty action used for keeping session alive when user pressed cancel logout.
    /// </summary>
    [HttpGet("Ping")]
    public IActionResult Ping()
        => Ok();
}
