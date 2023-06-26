using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OneBeyond.Studio.FeaturePermissions;
public static class IdentityExtensions
{
    public static string GetUserId(this IIdentity identity)
            => identity.GetFromClaim(ApplicationClaims.ApplicationUserId);

    public static string GetUserType(this IIdentity identity)
        => identity.GetFromClaim(ApplicationClaims.ApplicationUserType);

    public static string? GetUserRole(this IIdentity identity)
        => identity.GetFromClaim(ApplicationClaims.ApplicationUserRole);

    public static string GetFromClaim(this IIdentity identity, string type)
        => (identity as ClaimsIdentity)?.FindFirst(type)?.Value ?? string.Empty;
}
