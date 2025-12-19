using System;
using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using EnsureThat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OneBeyond.Studio.Core.Mediator;
using OneBeyond.Studio.Hosting.AspNet.ModelBinders.MixedSource;
using OneBeyond.Studio.Obelisk.Application.Features.Users.Dto;
using OneBeyond.Studio.Obelisk.Authentication.Domain;
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
        _clientApplicationLinkGenerator = EnsureArg.IsNotNull(clientApplicationLinkGenerator, nameof(clientApplicationLinkGenerator));
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="dto">A <see cref="CreateUserDto"/> with the new user's data.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> token to cancel the operation.</param>
    /// <returns><see cref="CreatedAtActionResult"/> with the <see cref="Guid"/> of the new user.</returns>
    /// <response code="201">Returns the GUID of the new user.</response>
    /// <response code="400">If the request body is invalid.</response>
    [HttpPost()]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CreateUser(
        [FromBody] CreateUserDto dto,
        CancellationToken cancellationToken)
    {
        var newLogin = await Mediator.CommandAsync<CreateLogin, ResetPasswordToken>(
            new CreateLogin(
                dto.UserName,
                dto.Email,
                dto.RoleId),
            cancellationToken);

        var createCommand = new CreateUser(
            newLogin.LoginId,
            dto.UserName,
            dto.Email,
            dto.RoleId,
            _clientApplicationLinkGenerator.GetSetPasswordUrl(newLogin.LoginId, newLogin.Value));

        var result = await Mediator.CommandAsync<CreateUser, Guid>(createCommand, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result }, result);
    }

    // Please note! command parameter has FromMixedSource attribute!
    // That means this command will be assembled from different sources:
    //   - UpdateUser.UserId will be bound from query (userId)
    //   - other command properties (userName, email, roleId, isActive) will be taken from request body
    /// <summary>
    /// Updates a specified user.
    /// </summary>
    /// <param name="command">An <see cref="Domain.Features.Users.Commands.UpdateUser">UpdateUser</see> with the user's updated data.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> token to cancel the operation.</param>
    /// <returns><see cref="NoContentResult"/>.</returns>
    /// <response code="204">If the user has been updated successfully.</response>
    /// <response code="400">If the specified ID or the request body is invalid.</response>
    /// <response code="404">If the user does not exist.</response>
    [HttpPut("{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateUser(
        [FromMixedSource] UpdateUser command,
        CancellationToken cancellationToken)
    {
        await Mediator.CommandAsync(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Generates a reset password token for a specified login ID.
    /// </summary>
    /// <param name="loginId">The <see cref="UserBase.LoginId">login ID</see> of the user.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> token to cancel the operation.</param>
    /// <returns><see cref="NoContentResult"/>.</returns>
    /// <response code="204">If the token is generated successfully.</response>
    /// <response code="404">If a user with the specified login ID does not exist.</response>
    [HttpPut("{loginId}/ResetPassword")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ResetPassword(
        string loginId,
        CancellationToken cancellationToken)
    {
        var resetPasswordToken = await Mediator.CommandAsync<GenerateResetPasswordTokenByLoginId, string>(
            new GenerateResetPasswordTokenByLoginId(loginId), cancellationToken);

        await Mediator.CommandAsync(
            new SendResetPasswordEmail(
                loginId,
                _clientApplicationLinkGenerator.GetResetPasswordUrl(loginId, resetPasswordToken)),
            cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Unlocks a specified user.
    /// </summary>
    /// <param name="userId" example="3fa85f64-5717-4562-b3fc-2c963f66afa6">The ID of the user.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> token to cancel the operation.</param>
    /// <returns><see cref="NoContentResult"/>.</returns>
    /// <response code="204">If the user has been successfully unlocked.</response>
    /// <response code="404">If the user does not exist.</response>
    [HttpPut("{userId}/Unlock")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UnlockUser(Guid userId, CancellationToken cancellationToken)
    {
        await Mediator.CommandAsync(new UnlockUser(userId), cancellationToken);
        return NoContent();
    }
}
