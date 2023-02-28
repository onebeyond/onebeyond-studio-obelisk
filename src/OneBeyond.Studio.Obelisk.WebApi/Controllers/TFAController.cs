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
        => _mediator.Send(
            new GetTfaSettings(_userContext.UserAuthId), cancellationToken);

    [HttpPost("generateTfaKey")]
    public async Task<TfaAuthenticatorSettings> GenerateTfaKey(CancellationToken cancellationToken)
    {
        var authenticatorKey = await _mediator.Send(
            new GenerateTfaKey(_userContext.UserAuthId), cancellationToken).ConfigureAwait(false);

        return new TfaAuthenticatorSettings
        {
            AuthenticationUri = GenerateQrCodeUri(authenticatorKey.Email, authenticatorKey.RawKey),
            SharedKey = authenticatorKey.SharedKey
        };
    }

    [HttpPost("enableTfa")]
    public Task<IEnumerable<string>> EnableTfa(
        [FromBody] EnableTfaModel enableTfaDto,
        CancellationToken cancellationToken)
    => _mediator.Send(
        new EnableTfa(_userContext.UserAuthId, enableTfaDto.Code), cancellationToken);
    
    [HttpPost("disableTfa")]
    public Task DisableTfa(CancellationToken cancellationToken) 
        => _mediator.Send(
            new DisableTfa(_userContext.UserAuthId, disableAuthenticator: false), cancellationToken);

    [HttpPost("resetTfa")]
    public Task Reset(CancellationToken cancellationToken) 
        => _mediator.Send(
            new DisableTfa(_userContext.UserAuthId, disableAuthenticator: true), cancellationToken);

    [HttpPost("forgetBrowser")]
    public Task ForgetBrowser(CancellationToken cancellationToken)
        => _mediator.Send(
            new ForgetTfaClient(_userContext.UserAuthId), cancellationToken);

    [HttpPost("generateRecoveryCodes")]
    public Task<IEnumerable<string>> GenerateRecoveryCodes(CancellationToken cancellationToken)
        => _mediator.Send(
            new GenerateTfaRecoveryCodes(_userContext.UserAuthId), cancellationToken);

    private string GenerateQrCodeUri(string email, string unformattedKey)
        => string.Format(
            AUTHENTICATOR_URI_FORMAT,
            _urlEncoder.Encode("Obelisk"),
            _urlEncoder.Encode(email),
            unformattedKey);
}
