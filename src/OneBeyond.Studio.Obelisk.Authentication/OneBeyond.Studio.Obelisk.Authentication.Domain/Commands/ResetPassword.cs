using EnsureThat;
using MediatR;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;

public sealed record ResetPassword : IRequest<Unit>
{
    public ResetPassword(
        string userName,
        string resetPasswordCode,
        string password)
    {
        EnsureArg.IsNotNullOrWhiteSpace(userName, nameof(userName));
        EnsureArg.IsNotNullOrWhiteSpace(resetPasswordCode, nameof(resetPasswordCode));
        EnsureArg.IsNotNullOrWhiteSpace(password, nameof(password));

        UserName = userName;
        ResetPasswordCode = resetPasswordCode;
        Password = password;
    }

    public string UserName { get; }
    public string ResetPasswordCode { get; }
    public string Password { get; }
}
