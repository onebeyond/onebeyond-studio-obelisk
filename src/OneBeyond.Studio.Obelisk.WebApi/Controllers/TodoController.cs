using System;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OneBeyond.Studio.Hosting.AspNet.ModelBinders.MixedSource;
using OneBeyond.Studio.Obelisk.Application.Features.Users.Dto;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;
using OneBeyond.Studio.Obelisk.Domain.Features.Examples.Commands;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Commands;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Entities;
using OneBeyond.Studio.Obelisk.WebApi.Helpers;
using OneBeyond.Studio.Obelisk.WebApi.Requests.Auth;

namespace OneBeyond.Studio.Obelisk.WebApi.Controllers;

[Authorize(Roles = UserRole.ADMINISTRATOR)]
[Produces("application/json")]
[ApiVersion("1.0")]
public sealed class TodoController : ControllerBase
{
    private readonly IMediator _mediator;

    public TodoController(
        IMediator mediator)
    {
        _mediator = EnsureArg.IsNotNull(mediator, nameof(mediator));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("BulkInsert/{count}")]
    public Task BulkInsert(int count, CancellationToken cancellationToken)
        => _mediator.Send(new BulkInsert(count), cancellationToken);
}
