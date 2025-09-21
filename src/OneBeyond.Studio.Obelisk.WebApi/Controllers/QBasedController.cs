using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OneBeyond.Studio.Application.SharedKernel.Entities.Dto;
using OneBeyond.Studio.Application.SharedKernel.Entities.Queries;
using OneBeyond.Studio.Domain.SharedKernel.Entities;
using OneBeyond.Studio.Obelisk.WebApi.Extensions;
using OneBeyond.Studio.Obelisk.WebApi.Helpers;
using OneBeyond.Studio.Obelisk.WebApi.Requests;

namespace OneBeyond.Studio.Obelisk.WebApi.Controllers;

public abstract class QBasedController<TEntityGetDTO, TEntityListDTO, TEntity, TEntityId>
    : ControllerBase
    where TEntity : DomainEntity<TEntityId>
    where TEntityGetDTO : new()
    where TEntityListDTO : new()
{
    protected QBasedController(
        ILogger<QBasedController<TEntityGetDTO, TEntityListDTO, TEntity, TEntityId>> logger,
        IMediator mediator)
    {
        Logger = EnsureArg.IsNotNull(logger, nameof(logger));
        Mediator = EnsureArg.IsNotNull(mediator, nameof(mediator));
    }

    protected ILogger<QBasedController<TEntityGetDTO, TEntityListDTO, TEntity, TEntityId>> Logger { get; }

    protected IMediator Mediator { get; }

    /// <summary>
    /// Gets a list of entities.
    /// </summary>
    /// <param name="queryParameters">The <see cref="ListRequest"/> representation of the query parameters to apply.</param>
    /// <param name="query">TODO!</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> token to cancel the operation.</param>
    /// <returns><see cref="OkResult"/> with a <see cref="PagedList{TEntity}"/> of entities.</returns>
    /// <response code="200">Returns a paged list of entities.</response>
    /// <response code="400">If a query parameter is invalid.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public virtual async Task<ActionResult<PagedList<TEntityListDTO>>> Get(
        [FromQuery] ListRequest queryParameters,
        Dictionary<string, IReadOnlyCollection<string>> query,
        CancellationToken cancellationToken)
    {
        query = ControllerHelpers.CleanQuery(query);
        var request = queryParameters.ToListQuery<TEntityListDTO, TEntity, TEntityId>(query);
        var result = await Mediator.Send(request, cancellationToken);
        return result;
    }

    /// <summary>
    /// Gets a specified entity.
    /// </summary>
    /// <param name="id" example="3fa85f64-5717-4562-b3fc-2c963f66afa6">The ID of the entity.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> token to cancel the operation.</param>
    /// <returns><see cref="OkResult"/> with the <typeparamref name="TEntityGetDTO"/>.</returns>
    /// <response code="200">Returns the specified entity.</response>
    /// <response code="404">If the entity does not exist.</response>
    /// <response code="400">If the ID is invalid.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public virtual async Task<ActionResult<TEntityGetDTO>> GetById(TEntityId id, CancellationToken cancellationToken)
    {
        var request = new GetById<TEntityGetDTO, TEntity, TEntityId>(id);
        var result = await Mediator.Send(request, cancellationToken);
        return result;
    }
}
