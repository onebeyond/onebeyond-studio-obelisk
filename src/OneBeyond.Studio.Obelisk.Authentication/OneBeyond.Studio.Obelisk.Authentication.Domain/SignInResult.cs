namespace OneBeyond.Studio.Obelisk.Authentication.Domain;

public struct SignInResult
{
    public SignInResult(
        SignInStatus status,
        string message)
    {
        Status = status;
        StatusMessage = message;
    }

    public SignInStatus Status { get; }
    public string StatusMessage { get; }
}
