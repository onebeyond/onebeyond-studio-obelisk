using EnsureThat;
using MediatR;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;

public sealed record UpdateLogin : IRequest<UpdateLoginResult>
{
    public UpdateLogin(
        string loginId,
        string userName,
        string email,
        string? roleId)
    {
        EnsureArg.IsNotNullOrWhiteSpace(loginId, nameof(loginId));
        EnsureArg.IsNotNullOrWhiteSpace(userName, nameof(userName));
        EnsureArg.IsNotNullOrWhiteSpace(email, nameof(email));

        LoginId = loginId;
        UserName = userName;
        Email = email;
        RoleId = roleId;
    }

    public string LoginId { get; }
    public string UserName { get; }
    public string Email { get; }
    public string? RoleId { get; }
}
