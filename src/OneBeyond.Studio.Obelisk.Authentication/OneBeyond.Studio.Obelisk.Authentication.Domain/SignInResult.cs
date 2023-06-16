namespace OneBeyond.Studio.Obelisk.Authentication.Domain;

public struct SignInResult
{
    public SignInResult(
        SignInStatus status,
        int? lockoutMinutes = null)
    {
        Status = status;
        LockoutMinutes = lockoutMinutes;
    }

    public SignInStatus Status { get; }
    public int? LockoutMinutes { get; }
}
