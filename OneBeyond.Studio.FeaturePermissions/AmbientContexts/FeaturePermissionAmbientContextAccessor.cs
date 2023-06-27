using System.Security.Claims;
using EnsureThat;
using Microsoft.AspNetCore.Http;
using OneBeyond.Studio.Application.SharedKernel.AmbientContexts;
using OneBeyond.Studio.Crosscuts.Utilities.Identities;
using OneBeyond.Studio.Hosting.AspNet.Http;

namespace OneBeyond.Studio.FeaturePermissions.AmbientContexts;

/// <summary>
/// Ambient context accessor with feature permissions. For use where the web ambient context is available.
/// </summary>
public sealed class FeaturePermissionAmbientContextAccessor : IAmbientContextAccessor<AmbientContext>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FeaturePermissionAmbientContextAccessor(IHttpContextAccessor httpContextAccessor)
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
            new FeaturePermissionUserContext(                
                userIdentity.TryGetLoginId()!,
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
