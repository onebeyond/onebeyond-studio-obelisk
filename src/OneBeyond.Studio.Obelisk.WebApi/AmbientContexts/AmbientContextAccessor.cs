using System;
using System.Security.Claims;
using EnsureThat;
using Microsoft.AspNetCore.Http;
using OneBeyond.Studio.Application.SharedKernel.AmbientContexts;
using OneBeyond.Studio.Hosting.AspNet.Http;
using OneBeyond.Studio.Obelisk.Application.Services.AmbientContexts;
using AmbientContext = OneBeyond.Studio.Obelisk.Application.Services.AmbientContexts.AmbientContext;

namespace OneBeyond.Studio.Obelisk.WebApi.AmbientContexts;

internal sealed class AmbientContextAccessor : IAmbientContextAccessor<AmbientContext>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AmbientContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        EnsureArg.IsNotNull(httpContextAccessor, nameof(httpContextAccessor));

        _httpContextAccessor = httpContextAccessor;
    }

    public AmbientContext AmbientContext => BuildAmbientContext(_httpContextAccessor);

    private static AmbientContext BuildAmbientContext(IHttpContextAccessor httpContextAccessor)
    {
        var userIdentity = httpContextAccessor.HttpContext?.User.Identity;

        var userContext = userIdentity is null || !userIdentity.IsAuthenticated
            ? null
            : new UserContext(
                httpContextAccessor.GetUserAuthId(ClaimTypes.NameIdentifier), // Claim type depends on authentication method in use. For example, ot might be ClaimConstants.ObjectId for pure OpenId
                Guid.Parse(userIdentity.GetUserId()),
                userIdentity.GetUserType(),
                userIdentity.GetUserRole());

        return new AmbientContext(userContext);
    }
}
