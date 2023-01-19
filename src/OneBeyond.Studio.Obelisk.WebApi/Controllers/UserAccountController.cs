using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneBeyond.Studio.Crosscuts.Utilities.Identities;
using OneBeyond.Studio.Obelisk.Authentication.Domain;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;
using SignInResult = OneBeyond.Studio.Obelisk.Authentication.Domain.SignInResult;

namespace OneBeyond.Studio.Obelisk.WebApi.Controllers;

[Produces("application/json")]
[ApiVersion("1.0")]
public sealed class UserAccountController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserAccountController(IMediator mediator)
    {
        EnsureArg.IsNotNull(mediator, nameof(mediator));

        _mediator = mediator;
    }

    [HttpPost("ChangePassword")]
    public async Task<ChangePasswordResult> ChangePassword([FromBody] ChangePassword changePassword, CancellationToken cancellationToken)
    {
        changePassword.AttachLoginId(HttpContext.User?.Identity?.TryGetLoginId() ?? string.Empty);

        return await _mediator.Send(changePassword, cancellationToken).ConfigureAwait(false);
    }
}
