using EnsureThat;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain;

public struct ResetPasswordToken
{
    public ResetPasswordToken(string loginId, string token)
    {
        EnsureArg.IsNotNullOrWhiteSpace(loginId, nameof(loginId));
        EnsureArg.IsNotNullOrWhiteSpace(token, nameof(token));

        LoginId = loginId;
        Value = token;
    }

    public string LoginId { get; }
    public string Value { get; }
}
