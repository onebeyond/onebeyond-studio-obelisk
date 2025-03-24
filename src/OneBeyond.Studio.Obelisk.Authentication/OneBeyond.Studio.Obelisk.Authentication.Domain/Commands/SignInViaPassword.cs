using System;
using EnsureThat;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.Commands;

[Obsolete("This relies on cross-origin cookies, which are now deprecated in some browsers")]
public sealed record SignInViaPassword : SignIn
{
    public SignInViaPassword(
        string userName,
        string password,
        bool rememberMe)
    {
        EnsureArg.IsNotNullOrWhiteSpace(userName, nameof(userName));
        EnsureArg.IsNotNullOrWhiteSpace(password, nameof(userName));

        UserName = userName;
        Password = password;
        RememberMe = rememberMe;
    }

    public string UserName { get; }
    public string Password { get; }
    public bool RememberMe { get; }
}
