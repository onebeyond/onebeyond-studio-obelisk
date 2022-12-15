using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.Services.ApplicationClaims;

public interface IApplicationClaimsService
{
    Task<IReadOnlyCollection<Claim>> ListApplicationClaimsForUserAsync(string userLoginId, CancellationToken cancellationToken);
}
