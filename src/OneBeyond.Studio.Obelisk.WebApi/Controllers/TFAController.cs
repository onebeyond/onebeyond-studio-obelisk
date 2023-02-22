using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OneBeyond.Studio.Application.SharedKernel.AmbientContexts;
using OneBeyond.Studio.Obelisk.Application.Services.AmbientContexts;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Commands;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Exceptions;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Queries;
using OneBeyond.Studio.Obelisk.WebApi.Models.TFA;
using AmbientContext = OneBeyond.Studio.Obelisk.Application.Services.AmbientContexts.AmbientContext;

namespace OneBeyond.Studio.Obelisk.WebApi.Controllers;

[Produces("application/json")]
[ApiVersion("1.0")]
public class TFAController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly UserContext _userContext;
    private readonly UrlEncoder _urlEncoder;
    private const string AUTHENTICATOR_URI_FORMAT = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

    [TempData]
    public string[] RecoveryCodes { get; set; }

    public TFAController(IMediator mediator,
        IAmbientContextAccessor<AmbientContext> ambientContextAccessor,
        UrlEncoder urlEncoder)
    {
        EnsureArg.IsNotNull(mediator, nameof(mediator));
        EnsureArg.IsNotNull(ambientContextAccessor, nameof(ambientContextAccessor));
        EnsureArg.IsNotNull(urlEncoder, nameof(urlEncoder));

        _mediator = mediator;
        _userContext = ambientContextAccessor.AmbientContext.GetUserContext();
        _urlEncoder = urlEncoder;

        RecoveryCodes = Array.Empty<string>();
    }

    [HttpGet("disable2fa")]
    public async Task<Unit> Disable2FA(
        CancellationToken cancellationToken
        ) => await _mediator.Send(
                            new DisableTfa(_userContext.UserAuthId, disableAuthenticator: false), cancellationToken)
                            .ConfigureAwait(false);

    [HttpGet("tfaSettings")]
    public async Task<LoginTfaSettings> GetTfaSettings(CancellationToken cancellationToken)
        => await _mediator.Send(
                new GetTfaSettings(_userContext.UserAuthId),
                cancellationToken)
            .ConfigureAwait(false);

    [HttpGet("tfaKey")]
    public async Task<IActionResult> GetTfaKey(CancellationToken cancellationToken)
    {
        var authenticatorKey = await _mediator.Send(
                 new GenerateTfaKey(_userContext.UserAuthId), cancellationToken)
             .ConfigureAwait(false);

        return Ok(new EnableAuthenticatorSettings()
        {
            AuthenticationUri = GenerateQrCodeUri(authenticatorKey.Email, authenticatorKey.RawKey),
            SharedKey = authenticatorKey.SharedKey
        });
    }

    [HttpGet("resetAuth")]
    public async Task<Unit> Reset(
        CancellationToken cancellationToken
        ) => await _mediator.Send(
                new DisableTfa(_userContext.UserAuthId, disableAuthenticator: true), cancellationToken)
            .ConfigureAwait(false);

    [HttpGet("forgetBrowser")]
    public async Task<Unit> ForgetBrowser(
        CancellationToken cancellationToken
        )
        => await _mediator.Send(
                new ForgetTfaClient(_userContext.UserAuthId), cancellationToken)
            .ConfigureAwait(false);

    [HttpPost("verify")]
    public async Task<IActionResult> Verify(
        [FromBody] InputModel input,
        CancellationToken cancellationToken)
    {
        var verificationCode = input.Code.Replace(" ", string.Empty).Replace("-", string.Empty); // Strip spaces and hypens

        try
        {
            await _mediator.Send(
                    new EnableTfa(_userContext.UserAuthId, verificationCode), cancellationToken)
                .ConfigureAwait(false);

            var loginTFASettings = await _mediator.Send(
                    new GetTfaSettings(_userContext.UserAuthId))
                .ConfigureAwait(false);

            if (loginTFASettings.RecoveryCodesLeft == 0)
            {
                RecoveryCodes = (await _mediator.Send(
                        new GenerateTfaRecoveryCodes(_userContext.UserAuthId))
                    .ConfigureAwait(false))
                    .ToArray();

                return Ok(RecoveryCodes);
            }
            else
            {
                return NoContent();
            }
        }
        catch (InvalidTfaTokenException)
        {
            return BadRequest();
        }

    }

    [HttpPost("generateRecoveryCodes")]
    public async Task<IActionResult> GenerateRecoveryCodes(
      CancellationToken cancellationToken)
    {
        try
        {
            RecoveryCodes = (await _mediator.Send(
                new GenerateTfaRecoveryCodes(_userContext.UserAuthId), cancellationToken)
            .ConfigureAwait(false))
            .ToArray();

            return Ok();
        }
        catch (InvalidTfaTokenException)
        {
            return BadRequest("error");
        }
    }

    [HttpGet("getRecoveryCodes")]
    public IActionResult GetRecoveryCodes()
    {
        return Ok(RecoveryCodes);
    }

    private string GenerateQrCodeUri(string email, string unformattedKey)
        => string.Format(
    AUTHENTICATOR_URI_FORMAT,
            _urlEncoder.Encode("Obelisk"),
            _urlEncoder.Encode(email),
            unformattedKey);
}
