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
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        await _mediator.Send(new SignOut(), cancellationToken).ConfigureAwait(false);

        return RedirectToPage("/Account/SignedOut");
    }

    /// <summary>
    /// Empty action used for keeping session alive when user pressed cancel logout.
    /// </summary>
    [HttpGet]
    public IActionResult Ping()
    {
        return Ok();
    }
}
