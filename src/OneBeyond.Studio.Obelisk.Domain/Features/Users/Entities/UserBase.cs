using System;
using EnsureThat;
using OneBeyond.Studio.Domain.SharedKernel.Entities;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Commands;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.DomainEvents;

namespace OneBeyond.Studio.Obelisk.Domain.Features.Users.Entities;

public abstract class UserBase : AggregateRoot<Guid> 
{
    protected UserBase(
        string loginId,
        string userName,
        string email,
        string typeId,
        string? roleId,
        Uri? resetPasswordUrl
        )
        : base(Guid.NewGuid())
    {
        EnsureArg.IsNotNullOrWhiteSpace(loginId, nameof(loginId));
        EnsureArg.IsNotNullOrWhiteSpace(typeId, nameof(typeId));
        EnsureArg.IsNotNullOrWhiteSpace(userName, userName);
        EnsureArg.IsNotNullOrWhiteSpace(email, email);

        LoginId = loginId;
        TypeId = typeId;
        UserName = userName;
        Email = email;
        RoleId = roleId;
        IsActive = true;

        // This will raise both a domain event and an integration event
        RaiseIntegrationEvent(
            new UserCreated(Id, resetPasswordUrl), 
            raiseAsDomainEvent: true);
    }

#nullable disable
    protected UserBase()
    {
    }
#nullable restore

    /// <summary>
    /// Id of the identity user associated with this user
    /// </summary>
    public string LoginId { get; private set; }

    public string UserName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public bool IsLockedOut { get; private set; }

    public string TypeId { get; private set; } //Once assigned, user type _never_ changes
    public string? RoleId { get; private set; }

    public void Apply(UpdateUser command)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        UserName = command.UserName;
        Email = command.Email;
        RoleId = command.RoleId;
        IsActive = command.IsActive;
    }

    public void Lock()
        => IsLockedOut = true;

    public void Unlock()
        => IsLockedOut = false;
}
