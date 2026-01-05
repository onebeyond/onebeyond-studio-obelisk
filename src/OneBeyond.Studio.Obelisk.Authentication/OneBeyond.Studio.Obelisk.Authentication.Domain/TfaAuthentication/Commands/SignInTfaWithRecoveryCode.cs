using EnsureThat;
using OneBeyond.Studio.Core.Mediator.Commands;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Commands;

public sealed class SignInTfaWithRecoveryCode : ICommand<SignInWithRecoveryCodeResult>
{
    public SignInTfaWithRecoveryCode(string recoveryCode)
    {
        EnsureArg.IsNotNullOrWhiteSpace(recoveryCode, nameof(recoveryCode));

        RecoveryCode = recoveryCode;
    }

    public string RecoveryCode { get; }
}
