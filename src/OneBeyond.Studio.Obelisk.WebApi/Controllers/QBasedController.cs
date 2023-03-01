using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OneBeyond.Studio.Application.SharedKernel.Entities.Dto;
using OneBeyond.Studio.Application.SharedKernel.Entities.Queries;
using OneBeyond.Studio.Domain.SharedKernel.Entities;
using OneBeyond.Studio.Obelisk.WebApi.Extensions;
using OneBeyond.Studio.Obelisk.WebApi.Helpers;
using OneBeyond.Studio.Obelisk.WebApi.Requests;

namespace OneBeyond.Studio.Obelisk.WebApi.Controllers;

public abstract class QBasedController<TAggregateRootGetDTO, TAggregateRootListDTO, TAggregateRoot, TAggregateRootId>
    : ControllerBase
    where TAggregateRoot : DomainEntity<TAggregateRootId>, IAggregateRoot
    where TAggregateRootGetDTO : new()
    where TAggregateRootListDTO : new()
{
    protected QBasedController(IMediator mediator)
    {
        EnsureArg.IsNotNull(mediator, nameof(mediator));

        Mediator = mediator;
    }

    protected IMediator Mediator { get; }

    /// <summary>
    /// Gets a list of entities.
    /// </summary>
    /// <param name="queryParameters"></param>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">If the list of entities returned</response>
    /// <response code="400">If the request is invalid</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet]
    public virtual async Task<IActionResult> Get(
        [FromQuery] ListRequest queryParameters,
        Dictionary<string, IReadOnlyCollection<string>> query,
        CancellationToken cancellationToken)
    {
        query = ControllerHelpers.CleanQuery(query);
        var result = await ListAsync(queryParameters, query, cancellationToken);
        return Json(result);
    }

    /// <summary>
    /// Gets a specified entity.
    /// </summary>
    /// <param name="id" example="3fa85f64-5717-4562-b3fc-2c963f66afa6">The ID of the entity</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">If the entity is returned</response>
    /// <response code="400">If the entity with the specified ID does not exist</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("{id}")]
    public virtual async Task<IActionResult> GetById(
        TAggregateRootId id,
        CancellationToken cancellationToken)
    {
        var result = await GetByIdAsync<TAggregateRootGetDTO>(id, cancellationToken);
        return result is not null
            ? Json(result)
            : BadRequest("Not found");
    }

    protected virtual Task<PagedList<TAggregateRootListDTO>> ListAsync(
        ListRequest queryParameters,
        Dictionary<string, IReadOnlyCollection<string>> query,
        CancellationToken cancellationToken)
        => Mediator.Send(
            queryParameters.ToListQuery<TAggregateRootListDTO, TAggregateRoot, TAggregateRootId>(query),
            cancellationToken);

    protected virtual Task<TDTO> GetByIdAsync<TDTO>(
        TAggregateRootId aggregateRootId,
        CancellationToken cancellationToken)
        => Mediator.Send(
            new GetById<TDTO, TAggregateRoot, TAggregateRootId>(aggregateRootId),
            cancellationToken);
}

public abstract class QBasedController<TAggregateRootListDTO, TAggregateRoot, TAggregateRootId>
    : QBasedController<TAggregateRootListDTO, TAggregateRootListDTO, TAggregateRoot, TAggregateRootId>
    where TAggregateRoot : DomainEntity<TAggregateRootId>, IAggregateRoot
    where TAggregateRootListDTO : new()
{
    protected QBasedController(
        IMediator mediator)
        : base(mediator)
    {
    }
}
