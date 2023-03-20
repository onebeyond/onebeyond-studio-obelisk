using EnsureThat;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication;

public sealed record SignInWithRecoveryCodeResult
{
    public SignInWithRecoveryCodeResult(
        string loginId,
        SignInStatus status,
        string message)
    {
        EnsureArg.IsNotNull(loginId, nameof(loginId));

        LoginId = loginId;
        Status = status;
        StatusMessage = message;
    }

    public string LoginId { get; }
    public SignInStatus Status { get; }
    public string StatusMessage { get; }
}
