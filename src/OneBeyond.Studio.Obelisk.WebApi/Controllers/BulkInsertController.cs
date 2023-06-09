using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OneBeyond.Studio.Obelisk.Domain.Features.Examples.Commands;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Entities;

namespace OneBeyond.Studio.Obelisk.WebApi.Controllers;

[Authorize(Roles = UserRole.ADMINISTRATOR)]
[Produces("application/json")]
[ApiVersion("1.0")]
public sealed class BulkInsertController : ControllerBase
{
    private readonly IMediator _mediator;

    public BulkInsertController(
        IMediator mediator)
    {
        _mediator = EnsureArg.IsNotNull(mediator, nameof(mediator));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("Todos/{count}")]
    public Task BulkInsertTodos(int count, CancellationToken cancellationToken)
        => _mediator.Send(new BulkInsertTodoItems(count), cancellationToken);

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("IntThingies/{count}")]
    public Task BulkInsertIntThingies(int count, CancellationToken cancellationToken)
        => _mediator.Send(new BulkInsertIntThingies(count), cancellationToken);

}
