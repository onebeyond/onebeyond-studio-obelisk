using System;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneBeyond.Studio.Obelisk.Domain.Features.Examples.Commands;

namespace OneBeyond.Studio.Obelisk.WebApi.Controllers;

[Authorize]
[Produces("application/json")]
[ApiVersion("1.0")]
public sealed class ExamplesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExamplesController(
        IMediator mediator)
    {
        EnsureArg.IsNotNull(mediator, nameof(mediator));
        _mediator = mediator;
    }

    [HttpPost("TodoItems")]
    public Task<Guid> CreateTodoItem(
        [FromBody] CreateTodoItem command,
        CancellationToken cancellationToken)
        => _mediator.Send(command, cancellationToken);

    [HttpPut("TodoItems/Update")]
    public Task UpdateTodoItem(
        [FromBody] UpdateTodoItem command,
        CancellationToken cancellationToken)
        => _mediator.Send(command, cancellationToken);

    [HttpPut("TodoItems/Assign")]
    public Task AssignTodoItem(
        [FromBody] AssignTodoItem command,
        CancellationToken cancellationToken)
        => _mediator.Send(command, cancellationToken);

    [HttpPut("TodoItems/Complete")]
    public Task CompleteTodoItem(
        [FromBody] CompleteTodoItem command,
        CancellationToken cancellationToken)
        => _mediator.Send(command, cancellationToken);

    [HttpDelete("TodoItems")]
    public Task DeleteTodoItem(
        [FromBody] DeleteTodoItem command,
        CancellationToken cancellationToken)
        => _mediator.Send(command, cancellationToken);

    [HttpPost("TodoItems/Properties")]
    public Task<Guid> CreateTodoItemProperty(
        [FromBody] CreateTodoItemProperty command,
        CancellationToken cancellationToken)
        => _mediator.Send(command, cancellationToken);

    [HttpPut("TodoItems/Properties")]
    public Task UpdateTodoItemProperty(
        [FromBody] UpdateTodoItemProperty command,
        CancellationToken cancellationToken)
        => _mediator.Send(command, cancellationToken);

    [HttpDelete("TodoItems/Properties")]
    public Task DeleteTodoItemProperty(
        [FromBody] DeleteTodoItemProperty command,
        CancellationToken cancellationToken)
        => _mediator.Send(command, cancellationToken);

}
