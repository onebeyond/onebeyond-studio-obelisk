using System.Threading;
using System.Threading.Tasks;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.Services.JwtAuthentication;
public interface IJwtCleardownService
{
    /// <summary>
    /// Clears down tokens expired over a day ago. Prevents excessive table growth.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task ClearDownJwtTokensAsync(CancellationToken cancellationToken);
}
