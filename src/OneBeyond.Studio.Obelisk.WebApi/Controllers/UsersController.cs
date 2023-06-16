using System;
using System.Linq;
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
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Commands;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Entities;
using OneBeyond.Studio.Obelisk.WebApi.Helpers;

namespace OneBeyond.Studio.Obelisk.WebApi.Controllers;

[Authorize(Roles = UserRole.ADMINISTRATOR)]
[Produces("application/json")]
[ApiVersion("1.0")]
public sealed class UsersController : QBasedController<GetUserDto, ListUsersDto, UserBase, Guid>
{
    private readonly ClientApplicationLinkGenerator _clientApplicationLinkGenerator;

    public UsersController(
        IMediator mediator,
        ClientApplicationLinkGenerator clientApplicationLinkGenerator)
        : base(mediator)
    {
        EnsureArg.IsNotNull(mediator, nameof(mediator));
        EnsureArg.IsNotNull(clientApplicationLinkGenerator, nameof(clientApplicationLinkGenerator));

        _clientApplicationLinkGenerator = clientApplicationLinkGenerator;
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>  // Swagger ignores this
    /// <response code="200">If the GUID of the new user is returned</response>
    /// <response code="400">If the DTO is invalid</response>
    [ProducesResponseType(typeof(Guid), 200)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost()]
    public async Task<Guid> CreateUser(
        [FromBody] CreateUserDto dto,
        CancellationToken cancellationToken)
    {
        var newLogin = await Mediator.Send(
            new CreateLogin(
                dto.UserName,
                dto.Email,
                dto.RoleId
            ),
            cancellationToken).ConfigureAwait(false);

        var createCommand = new CreateUser(
            newLogin.LoginId,
                dto.UserName,
                dto.Email,
                dto.RoleId,
                _clientApplicationLinkGenerator.GetSetPasswordUrl(newLogin.LoginId, newLogin.Value)
        );

        return await Mediator.Send(createCommand, cancellationToken).ConfigureAwait(false);
    }

    //Please note! command parameter has FromMixedSource attribute!
    //That means this command will be assembled from different sources:
    // - UpdateUser.UserId will be bound from query (userId)
    // - other command properties (userName, email, role, isActive) will be taken from request body
    /// <summary>
    /// Updates a specified user.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">If the user has been updated successfully</response>
    /// <response code="400">If the user does not exist</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPut("{userId}")]
    public Task UpdateUser(
        [FromMixedSource] UpdateUser command,
        CancellationToken cancellationToken)
        => Mediator.Send(command, cancellationToken);

    /// <summary>
    /// Generates a reset password token for a specified login ID.
    /// </summary>
    /// <param name="loginId">The login Id of the user</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">If the token is generated successfully</response>
    /// <response code="400">If the login ID does not exist</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPut("{loginId}/ResetPassword")]
    public async Task ResetPassword(
        string loginId,
        CancellationToken cancellationToken)
    {
        var resetPasswordToken = await Mediator.Send(
            new GenerateResetPasswordTokenByLoginId(loginId), cancellationToken).ConfigureAwait(false);

        await Mediator.Send(
            new SendResetPasswordEmail(
                loginId,
                _clientApplicationLinkGenerator.GetResetPasswordUrl(loginId,resetPasswordToken)),
            cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Unlocks a specified user.
    /// </summary>
    /// <param name="userId" example="3fa85f64-5717-4562-b3fc-2c963f66afa6">The ID of the user</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">If the user has been unlocked successfully</response>
    /// <response code="400">If the user with the specified ID does not exist</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPut("{userId}/Unlock")]
    public Task UnlockUser(Guid userId, CancellationToken cancellationToken)
        => Mediator.Send(new UnlockUser(userId), cancellationToken);
}
