using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.EntityFrameworkCore;
using OneBeyond.Studio.Core.Mediator.Notifications;
using OneBeyond.Studio.Obelisk.Application.Services.Seeding;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Entities;

namespace OneBeyond.Studio.Obelisk.Infrastructure.Data.Seeding;

internal sealed class IdentitiesSeeder : INotificationHandler<SeedApplication>
{
    private readonly DomainContext _domainContext;
    private readonly string _adminPassword;

    public IdentitiesSeeder(
        DomainContext domainContext,
        IdentitiesSeederOptions options)
    {
        EnsureArg.IsNotNull(domainContext, nameof(domainContext));
        EnsureArg.IsNotNullOrWhiteSpace(options?.AdminPassword, nameof(options.AdminPassword));

        _domainContext = domainContext;
        _adminPassword = options.AdminPassword;
    }

    public async Task HandleAsync(SeedApplication notification, CancellationToken cancellationToken)
    {
        await AuthContextExtensions.SeedRolesAsync(_domainContext, cancellationToken).ConfigureAwait(false);
        await SeedAdminUserAsync(_domainContext, _adminPassword, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<User> SeedAdminUserAsync(
        DomainContext context,
        string adminPassword,
        CancellationToken cancellationToken)
    {
        const string adminUserName = "ObeliskAdmin";
        const string adminUserEmail = $"{adminUserName}@one-beyond.com";

        var adminUser = await context.Set<User>()
            .FirstOrDefaultAsync(
                user => user.UserName == adminUserName,
                cancellationToken)
            .ConfigureAwait(false);

        var adminLogin = await AuthContextExtensions.SeedLoginIfNotExistsAsync(
            context,
            adminUserName,
            adminUserEmail,
            adminPassword,
            UserRole.ADMINISTRATOR,
            cancellationToken).ConfigureAwait(false);

        if (adminUser is null)
        {
            adminUser = new User(adminLogin.Id, adminUserName, adminUserEmail, UserRole.ADMINISTRATOR, null);
            await context.Set<UserBase>().AddAsync(adminUser, cancellationToken).ConfigureAwait(false);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        return adminUser;
    }
}
