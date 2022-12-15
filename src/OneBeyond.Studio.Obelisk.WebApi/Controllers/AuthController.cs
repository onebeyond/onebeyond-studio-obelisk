using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;
using SignInResult = OneBeyond.Studio.Obelisk.Authentication.Domain.SignInResult;

namespace OneBeyond.Studio.Obelisk.WebApi.Controllers;

[Produces("application/json")]
[ApiVersion("1.0")]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        EnsureArg.IsNotNull(mediator, nameof(mediator));

        _mediator = mediator;
    }

    [HttpPost("Basic/SignIn")]
    public Task<SignInResult> BasicSignIn(
        [FromBody] SignInViaPassword signInViaPassword,
        CancellationToken cancellationToken)
        => _mediator.Send(signInViaPassword, cancellationToken);

    [Authorize]
    [HttpPost("SignOut")]
    public Task SignOut(CancellationToken cancellationToken)
        => _mediator.Send(new SignOut(), cancellationToken);

    /// <summary>
    /// Empty action used for keeping session alive when user pressed cancel logout.
    /// </summary>
    [HttpGet("Ping")]
    public IActionResult Ping()
        => Ok();
}
