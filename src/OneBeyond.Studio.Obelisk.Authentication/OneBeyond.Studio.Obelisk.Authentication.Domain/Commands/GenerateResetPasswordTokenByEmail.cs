using EnsureThat;
using MediatR;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;

public sealed record GenerateResetPasswordTokenByEmail : IRequest<ResetPasswordToken>
{
    public GenerateResetPasswordTokenByEmail(string email)
    {
        EnsureArg.IsNotNullOrWhiteSpace(email, nameof(email));

        Email = email;
    }

    public string Email { get; }
}
