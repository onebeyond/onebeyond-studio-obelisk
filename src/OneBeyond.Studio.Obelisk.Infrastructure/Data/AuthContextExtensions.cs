using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using OneBeyond.Studio.Obelisk.Authentication.Application.Entities;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Entities;

namespace OneBeyond.Studio.Obelisk.Infrastructure.Data;

public static class AuthContextExtensions
{
    public static async Task SeedRolesAsync(
        DomainContext context,
        CancellationToken cancellationToken)
    {
        var roleStore = new RoleStore<AuthRole, DomainContext>(context);
        using (var rm = new RoleManager<AuthRole>(roleStore, null!, null!, null!, null!))
        {
            await SeedRoleAsync(rm, UserRole.ADMINISTRATOR, cancellationToken).ConfigureAwait(false);
            await SeedRoleAsync(rm, UserRole.USER, cancellationToken).ConfigureAwait(false);
        }
    }

    public static async Task<AuthUser> SeedLoginIfNotExistsAsync(
        DomainContext context,
        string userName,
        string email,
        string password,
        string role,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var userStore = new UserStore<AuthUser, AuthRole, DomainContext>(context);

        using (var um = new UserManager<AuthUser>(userStore, null!, new PasswordHasher<AuthUser>(), null!, null!, null!, null!, null!, null!))
        {
            var user = await um.FindByNameAsync(userName).ConfigureAwait(false);

            if (user is null)
            {
                user = new AuthUser()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = userName,
                    Email = email,
                    AccessFailedCount = 0,
                    LockoutEnabled = false
                };

                await um.CreateAsync(user, password).ConfigureAwait(false);
                await um.AddToRoleAsync(user, role).ConfigureAwait(false);
            }

            return user;
        }
    }

    private static async Task SeedRoleAsync(
        RoleManager<AuthRole> roleManager,
        string role,
        CancellationToken cancellationToken)
    {
        var existingRole = await roleManager.FindByNameAsync(role).ConfigureAwait(false);

        cancellationToken.ThrowIfCancellationRequested();

        if (existingRole is null)
        {
            await roleManager.CreateAsync(new AuthRole(role)).ConfigureAwait(false);
        }
    }
}
