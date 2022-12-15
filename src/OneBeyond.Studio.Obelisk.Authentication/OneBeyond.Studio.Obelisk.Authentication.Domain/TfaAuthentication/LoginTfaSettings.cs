namespace OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication;

public struct LoginTfaSettings
{
    public LoginTfaSettings(
        bool hasAuthenticator,
        bool is2faEnabled,
        bool isMachineRemembered,
        int recoveryCodesLeft
        )
    {
        HasAuthenticator = hasAuthenticator;
        Is2faEnabled = is2faEnabled;
        IsMachineRemembered = isMachineRemembered;
        RecoveryCodesLeft = recoveryCodesLeft;
    }

    public bool HasAuthenticator { get; }
    public bool Is2faEnabled { get; }
    public bool IsMachineRemembered { get; }
    public int RecoveryCodesLeft { get; }

}
