using System.Threading;
using System.Threading.Tasks;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.Repositories.AuthUsers;
public interface IAuthTokenRepository
{
    /// <summary>
    /// Clears down tokens expired over a day ago. Prevents excessive table growth.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task ClearDownJwtTokensAsync(CancellationToken cancellationToken);
}
