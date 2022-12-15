using EnsureThat;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;

public sealed record SignInViaPassword : SignIn
{
    public SignInViaPassword(
        string userName,
        string password,
        bool rememberMe,
        bool lockoutOnFailure = true)
    {
        EnsureArg.IsNotNullOrWhiteSpace(userName, nameof(userName));
        EnsureArg.IsNotNullOrWhiteSpace(password, nameof(userName));

        UserName = userName;
        Password = password;
        RememberMe = rememberMe;
        LockoutOnFailure = lockoutOnFailure;
    }

    public string UserName { get; }
    public string Password { get; }
    public bool RememberMe { get; }

    /// <summary>
    /// Set this property to true if you want a user account to be locked after a number of unsuccessful login attempts
    /// </summary>
    public bool LockoutOnFailure { get; }
}
