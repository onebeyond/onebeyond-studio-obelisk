using EnsureThat;
using OneBeyond.Studio.Core.Mediator.Commands;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;

public sealed record GenerateResetPasswordTokenByEmail : ICommand<ResetPasswordToken>
{
    public GenerateResetPasswordTokenByEmail(string email)
    {
        EnsureArg.IsNotNullOrWhiteSpace(email, nameof(email));

        Email = email;
    }

    public string Email { get; }
}
