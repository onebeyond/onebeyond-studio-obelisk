using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OneBeyond.Studio.Application.SharedKernel.Entities.Dto;
using OneBeyond.Studio.Hosting.AspNet.ModelBinders.MixedSource;
using OneBeyond.Studio.Obelisk.Application.Features.Users.Dto;
using OneBeyond.Studio.Obelisk.Domain.Features.Dummies.Commands;
using OneBeyond.Studio.Obelisk.Domain.Features.Dummies.Entities;
using OneBeyond.Studio.Obelisk.WebApi.Helpers;
using OneBeyond.Studio.Obelisk.WebApi.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace OneBeyond.Studio.Obelisk.WebApi.Controllers;

[Produces("application/json")]
[ApiVersion("1.0")]
public sealed class DummiesController : QBasedController<DummyDto, DummyDto, Dummy, Guid>
{
    public DummiesController(
        IMediator mediator)
        : base(mediator)
    {
    }

    /// <summary>
    /// Gets a list of entities.
    /// </summary>
    /// <param name="queryParameters"></param>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">If the list of entities returned</response>
    /// <response code="400">If the request is invalid</response>
    [ProducesResponseType(typeof(PagedList<DummyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] ListQueryParameters queryParameters,
        Dictionary<string, IReadOnlyCollection<string>> query,
        CancellationToken cancellationToken)
    {
        query = ControllerHelpers.CleanQuery(query);
        var result = await ListAsync(queryParameters, query, cancellationToken);
        return Json(result);
    }

    /// <summary>
    /// Creates a new dummy.
    /// </summary>
    /// <param name="createCommand"></param>
    /// <param name="cancellationToken"></param>  // Swagger ignores this
    /// <response code="200">If the GUID of the new dummy is returned</response>
    /// <response code="400">If the DTO is invalid</response>
    [ProducesResponseType(typeof(Guid), 200)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost()]
    public async Task<Guid> CreateDummy(
        [FromBody] CreateDummy createCommand,
        CancellationToken cancellationToken)
    {
        return await Mediator.Send(createCommand, cancellationToken).ConfigureAwait(false);
    }

    //Please note! command parameter has FromMixedSource attribute!
    //That means this command will be assembled from different sources:
    // - UpdateDummy.DummyId will be bound from query (dummyId)
    // - other command properties (dummyName, email, role, isActive) will be taken from request body
    /// <summary>
    /// Updates a specified dummy.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">If the dummy has been updated successfully</response>
    /// <response code="400">If the dummy does not exist</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPut("{id}")]
    public Task UpdateDummy(
        [FromMixedSource] UpdateDummy command,
        CancellationToken cancellationToken)
        => Mediator.Send(command, cancellationToken);

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpDelete("{id}")]
    public Task DeleteDummy(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
        => Mediator.Send(new DeleteDummy(id), cancellationToken);

}
