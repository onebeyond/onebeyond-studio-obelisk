namespace OneBeyond.Studio.Obelisk.Authentication.Domain;

public enum SignInStatus
{
    Success,
    LockedOut,
    RequiresVerification,
    Failure,
    UnknownUser,
    ConfigureTFA
}
