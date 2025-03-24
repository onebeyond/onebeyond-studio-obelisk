using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.EntityFrameworkCore;
using OneBeyond.Studio.Obelisk.Authentication.Application.Entities;
using OneBeyond.Studio.Obelisk.Authentication.Application.Repositories.AuthUsers;

namespace OneBeyond.Studio.Obelisk.Infrastructure.Data.AuthUsers.Repositories;
internal class AuthTokenRepository : IAuthTokenRepository
{
    private readonly DomainContext _domainContext;

    public AuthTokenRepository(DomainContext domainContext)
    {
        EnsureArg.IsNotNull(domainContext, nameof(domainContext));
        _domainContext = domainContext;
    }

    public async Task ClearDownJwtTokensAsync(CancellationToken cancellationToken)
    {
        await _domainContext.Set<AuthToken>()
            .Where(x => x.ExpiresOn < DateTimeOffset.UtcNow.AddDays(-1))
            .ExecuteDeleteAsync(cancellationToken);
    }
}
