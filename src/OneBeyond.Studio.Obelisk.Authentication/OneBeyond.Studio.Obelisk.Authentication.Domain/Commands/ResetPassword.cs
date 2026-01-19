using EnsureThat;
using OneBeyond.Studio.Core.Mediator;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;

public sealed record ResetPassword : IRequest<ResetPasswordStatus>
{
    public ResetPassword(
        string userId,
        string token,
        string password)
    {
        EnsureArg.IsNotNullOrWhiteSpace(userId, nameof(userId));
        EnsureArg.IsNotNullOrWhiteSpace(token, nameof(token));
        EnsureArg.IsNotNullOrWhiteSpace(password, nameof(password));

        UserId = userId;
        Token = token;
        Password = password;
    }
    public string UserId { get; }
    public string Token { get; }
    public string Password { get; }
}
