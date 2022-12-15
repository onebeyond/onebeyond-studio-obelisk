using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OneBeyond.Studio.Obelisk.Authentication.Application.Entities;

namespace OneBeyond.Studio.Obelisk.Infrastructure.Data;

//We use this store to be able to provide more properties for AuthUser, like lists of AuthTokens
public sealed class AuthUserStore : UserStore<AuthUser, AuthRole, DomainContext>
{
    public AuthUserStore(DomainContext context)
        : base(context)
    {
    }

    public override IQueryable<AuthUser> Users
        => base.Users.Include(x => x.AuthTokens);

    public override Task<AuthUser?> FindByIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
        => Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

    public override Task<AuthUser?> FindByNameAsync(
        string normalizedUserName,
        CancellationToken cancellationToken = default)
        => Users.FirstOrDefaultAsync(x => x.NormalizedUserName == normalizedUserName, cancellationToken);

    public override Task<AuthUser?> FindByEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken = default)
        => Users.FirstOrDefaultAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken);

    public override async Task<IdentityResult> UpdateAsync(AuthUser user, CancellationToken cancellationToken = default)
    {
        Context.Attach(user);

        FixUserTokenStatus(user);

        user.ConcurrencyStamp = Guid.NewGuid().ToString();

        Context.Update(user);

        await SaveChanges(cancellationToken);

        return IdentityResult.Success;
    }

    private void FixUserTokenStatus(AuthUser user)
    {
        foreach (var token in user.AuthTokens)
        {
            if (Context.Entry(token).State == EntityState.Detached)
            {
                Context.Add(token);
            }
        }
    }
}
