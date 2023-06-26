using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Http;
using OneBeyond.Studio.Crosscuts.Utilities.Identities;
using OneBeyond.Studio.Hosting.AspNet.Http;

namespace OneBeyond.Studio.FeaturePermissions.AmbientContexts;
public sealed class AmbientContextAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AmbientContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        EnsureArg.IsNotNull(httpContextAccessor, nameof(httpContextAccessor));

        _httpContextAccessor = httpContextAccessor;
    }

    public AmbientContext AmbientContext
        => BuildAmbientContext(_httpContextAccessor);

    private AmbientContext BuildAmbientContext(IHttpContextAccessor httpContextAccessor)
    {
        var claimsPrincipal = httpContextAccessor.HttpContext?.User;

        var userIdentity = claimsPrincipal?.Identity;

        var userContext = (userIdentity is null || !userIdentity.IsAuthenticated) ?
            null :
            new UserContext(
                httpContextAccessor.GetUserClaimValue(ClaimTypes.NameIdentifier),
                Guid.Parse(userIdentity.TryGetLoginId()!),
                userIdentity.GetUserType(),
                userIdentity.GetUserRole(),
                GetFeaturePermissions(claimsPrincipal));

        return new AmbientContext(userContext);
    }


    private static IEnumerable<string> GetFeaturePermissions(ClaimsPrincipal? identity)
        => identity is null ? (IEnumerable<string>)Array.Empty<string>() :
            identity.Claims.Where(x => x.Type == ApplicationClaims.ApplicationFeature)
                .Select(x => x.Value);

}
