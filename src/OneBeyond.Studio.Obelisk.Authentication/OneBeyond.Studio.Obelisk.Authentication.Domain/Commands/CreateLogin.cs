using EnsureThat;
using OneBeyond.Studio.Core.Mediator;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;

public sealed record CreateLogin : IRequest<ResetPasswordToken>
{
    public CreateLogin(
        string userName,
        string email,
        string? roleId,
        SignInProviderLogin? externalLogin = null)
    {
        EnsureArg.IsNotNullOrWhiteSpace(userName, nameof(userName));
        EnsureArg.IsNotNullOrWhiteSpace(email, nameof(email));

        UserName = userName;
        Email = email;
        RoleId = roleId;
        ExternalLogin = externalLogin;
    }

    public string UserName { get; }
    public string Email { get; }
    public string? RoleId { get; }
    public SignInProviderLogin? ExternalLogin { get; }
}
