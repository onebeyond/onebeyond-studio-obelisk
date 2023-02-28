using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneBeyond.Studio.Obelisk.Authentication.Domain.JwtAuthentication;
using OneBeyond.Studio.Obelisk.Authentication.Domain.JwtAuthentication.Commands;

namespace OneBeyond.Studio.Obelisk.WebApi.Controllers;

[AllowAnonymous]
[Produces("application/json")]
[Route("api/account/jwt")]
public sealed class JWTAuthenticationController : Controller
{
    public sealed record SignInJwtDto
    {
        public SignInJwtDto(
            string username,
            string password)
        {
            EnsureArg.IsNotNullOrWhiteSpace(username, nameof(username));
            EnsureArg.IsNotNullOrWhiteSpace(password, nameof(password));

            Username = username;
            Password = password;
        }

        public string Username { get; private init; }
        public string Password { get; private init; }
    }

    private readonly IMediator _mediator;

    public JWTAuthenticationController(IMediator mediator)
    {
        EnsureArg.IsNotNull(mediator, nameof(mediator));

        _mediator = mediator;
    }

    /// <summary>
    /// JWT Authenticate
    /// </summary>
    /// <remarks>
    /// Authenticates a user and returns a JWT token, which will be used to further web API requests the endpoints that require user authentication
    /// </remarks>
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
    [HttpPut("refreshToken")]
    public Task<JwtToken> RefreshToken(
        [FromBody] string refreshToken,
        CancellationToken cancellationToken)
        => _mediator.Send(
                new RefreshJwtToken(refreshToken),
                cancellationToken);
}
