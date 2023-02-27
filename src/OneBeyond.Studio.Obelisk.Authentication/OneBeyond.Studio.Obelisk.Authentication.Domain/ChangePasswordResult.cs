namespace OneBeyond.Studio.Obelisk.Authentication.Domain;

public sealed record ChangePasswordResult
{
    private ChangePasswordResult()
    {
        Success = true;
    }

    private ChangePasswordResult(
        string message)
    {
        Success = false;
        ErrorMessage = message;
    }

    public string? ErrorMessage { get; }

    public bool Success { get; }

    public static ChangePasswordResult SuccessResult()
        => new();

    public static ChangePasswordResult UnkownUserResult(string loginId) 
        => new($"Login with id {loginId} not found");

    public static ChangePasswordResult UserWithNoPasswordResult()
        => new($"Login has no a password yet, use SetPassword to set it");

    public static ChangePasswordResult OperationFailureResult(string message)
        => new(message);
}
