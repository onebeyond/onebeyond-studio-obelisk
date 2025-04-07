using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using OneBeyond.Studio.Obelisk.Authentication.Application.Repositories.AuthUsers;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.Services.JwtAuthentication;
internal class JwtCleardownServce : IJwtCleardownService
{
    private readonly IAuthTokenRepository _authTokenRepository;

    public JwtCleardownServce(IAuthTokenRepository authTokenRepository)
    {
        EnsureArg.IsNotNull(authTokenRepository, nameof(authTokenRepository));
        _authTokenRepository = authTokenRepository;
    }

    public Task ClearDownJwtTokensAsync(CancellationToken cancellationToken)
        => _authTokenRepository.ClearDownJwtTokensAsync(cancellationToken);
}
