using EnsureThat;
using MediatR;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;

public sealed record SetPassword : IRequest
{
    public SetPassword(
        string loginId,
        string resetPasswordCode,
        string password)
    {
        EnsureArg.IsNotNullOrWhiteSpace(loginId, nameof(loginId));
        EnsureArg.IsNotNullOrWhiteSpace(resetPasswordCode, nameof(resetPasswordCode));
        EnsureArg.IsNotNullOrWhiteSpace(password, nameof(password));

        LoginId = loginId;
        ResetPasswordCode = resetPasswordCode;
        Password = password;
    }

    public string LoginId { get; }
    public string ResetPasswordCode { get; }
    public string Password { get; }
}
