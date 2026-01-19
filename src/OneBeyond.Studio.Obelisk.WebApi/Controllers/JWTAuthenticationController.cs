using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneBeyond.Studio.Application.SharedKernel.AmbientContexts;
using OneBeyond.Studio.Core.Mediator;
using OneBeyond.Studio.Obelisk.Application.Services.AmbientContexts;
using OneBeyond.Studio.Obelisk.Authentication.Domain.JwtAuthentication;
using OneBeyond.Studio.Obelisk.Authentication.Domain.JwtAuthentication.Commands;
using AmbientContext = OneBeyond.Studio.Obelisk.Application.Services.AmbientContexts.AmbientContext;


namespace OneBeyond.Studio.Obelisk.WebApi.Controllers;

[Produces("application/json")]
[Route("api/account/jwt")]
public sealed class JWTAuthenticationController : Controller
{

    private readonly IMediator _mediator;
    private readonly AmbientContext _ambientContext;

    public JWTAuthenticationController(IMediator mediator, IAmbientContextAccessor<AmbientContext> ambientContextAccessor)
    {
        EnsureArg.IsNotNull(mediator, nameof(mediator));
        EnsureArg.IsNotNull(ambientContextAccessor, nameof(ambientContextAccessor));

        _mediator = mediator;
        _ambientContext = ambientContextAccessor.AmbientContext;
    }

    /// <summary>
    /// JWT Authenticate
    /// </summary>
    /// <remarks>
    /// Authenticates a user and returns a JWT token, which will be used to further web API requests the endpoints that require user authentication
    /// </remarks>
    [AllowAnonymous]
    [HttpPost("signIn")]
    public Task<JwtToken> Authenticate(
        [FromBody] SignInJwtDto credentials,
        CancellationToken cancellationToken)
        => _mediator.Send(
                new SignInJwtToken(
                    credentials.Username,
                    credentials.Password
                ),
                cancellationToken);

    /// <summary>
    /// Refresh Token
    /// </summary>
    /// <remarks>
    /// Refreshes the active token in order it to be valid for future requests.
    /// </remarks>
    [AllowAnonymous]
    [HttpPut("refreshToken")]
    public Task<JwtToken> RefreshToken(
        [FromBody] string refreshToken,
        CancellationToken cancellationToken)
        => _mediator.Send(
                new RefreshJwtToken(refreshToken),
                cancellationToken);

    [Authorize]
    [HttpPost("signout")]
    public Task SignOutAllTokens(CancellationToken cancellationToken)
        => _mediator.Send(new SignOutAllTokens(_ambientContext.GetUserContext().UserAuthId), cancellationToken);

}
