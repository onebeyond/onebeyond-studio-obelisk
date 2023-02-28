using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

[Authorize]
[Produces("application/json")]
[ApiVersion("1.0")]
public sealed class TFAController : ControllerBase
{
    private const string AUTHENTICATOR_URI_FORMAT = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

    private readonly IMediator _mediator;
    private readonly UserContext _userContext;
    private readonly UrlEncoder _urlEncoder;

    public TFAController(
        IMediator mediator,
        IAmbientContextAccessor<AmbientContext> ambientContextAccessor,
        UrlEncoder urlEncoder)
    {
        EnsureArg.IsNotNull(mediator, nameof(mediator));
        EnsureArg.IsNotNull(ambientContextAccessor, nameof(ambientContextAccessor));
        EnsureArg.IsNotNull(urlEncoder, nameof(urlEncoder));

        _mediator = mediator;
        _userContext = ambientContextAccessor.AmbientContext.GetUserContext();
        _urlEncoder = urlEncoder;
    }

    [HttpGet("tfaSettings")]
    public Task<LoginTfaSettings> GetTfaSettings(CancellationToken cancellationToken)
        => _mediator.Send(new GetTfaSettings(_userContext.UserAuthId), cancellationToken);

    [HttpGet("tfaKey")]
    public async Task<EnableAuthenticatorSettings> GetTfaKey(CancellationToken cancellationToken)
    {
        var authenticatorKey = await _mediator.Send(
                 new GenerateTfaKey(_userContext.UserAuthId), cancellationToken)
             .ConfigureAwait(false);

        return new EnableAuthenticatorSettings
        {
            AuthenticationUri = GenerateQrCodeUri(authenticatorKey.Email, authenticatorKey.RawKey),
            SharedKey = authenticatorKey.SharedKey
        };
    }

    [HttpPost("disable2fa")]
    public Task Disable2FA(CancellationToken cancellationToken) 
        => _mediator.Send(new DisableTfa(_userContext.UserAuthId, disableAuthenticator: false), cancellationToken);

    [HttpPost("resetAuth")]
    public Task Reset(CancellationToken cancellationToken) 
        => _mediator.Send(new DisableTfa(_userContext.UserAuthId, disableAuthenticator: true), cancellationToken);

    [HttpPost("forgetBrowser")]
    public Task ForgetBrowser(CancellationToken cancellationToken)
        => _mediator.Send(new ForgetTfaClient(_userContext.UserAuthId), cancellationToken);

    [HttpPost("generateRecoveryCodes")]
    public Task<IEnumerable<string>> GenerateRecoveryCodes(CancellationToken cancellationToken)
        => _mediator.Send(new GenerateTfaRecoveryCodes(_userContext.UserAuthId), cancellationToken);

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

            var loginTFASettings = await _mediator.Send(new GetTfaSettings(_userContext.UserAuthId)).ConfigureAwait(false);

            if (loginTFASettings.RecoveryCodesLeft == 0)
            {
                var recoveryCodes = (await _mediator.Send(
                        new GenerateTfaRecoveryCodes(_userContext.UserAuthId))
                    .ConfigureAwait(false))
                    .ToArray();

                return Ok(recoveryCodes);
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

    private string GenerateQrCodeUri(string email, string unformattedKey)
        => string.Format(
            AUTHENTICATOR_URI_FORMAT,
            _urlEncoder.Encode("Obelisk"),
            _urlEncoder.Encode(email),
            unformattedKey);
}
