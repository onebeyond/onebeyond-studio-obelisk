using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;

namespace OneBeyond.Studio.Obelisk.WebApi.Controllers;

[Route("[controller]/[action]")]
public sealed class AccountController : Controller
{
    private readonly IMediator _mediator;

    public AccountController(IMediator mediator)
    {
        EnsureArg.IsNotNull(mediator, nameof(mediator));

        _mediator = mediator;
    }

    [Authorize]
    [HttpPost]
    public Task Logout(CancellationToken cancellationToken)
        => _mediator.Send(new SignOut(), cancellationToken);

    /// <summary>
    /// Empty action used for keeping session alive when user pressed cancel logout.
    /// </summary>
    [HttpGet]
    public IActionResult Ping()
    {
        return Ok();
    }
}
