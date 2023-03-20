using EnsureThat;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication;

public sealed record TfaKey
{
    public TfaKey(
        string loginId,
        string email,
        string rawKey,
        string sharedKey)
    {
        EnsureArg.IsNotNullOrWhiteSpace(loginId, nameof(loginId));
        EnsureArg.IsNotNullOrWhiteSpace(email, nameof(email));
        EnsureArg.IsNotNullOrWhiteSpace(rawKey, nameof(rawKey));
        EnsureArg.IsNotNullOrWhiteSpace(sharedKey, nameof(sharedKey));

        LoginId = loginId;
        Email = email;
        RawKey = rawKey;
        SharedKey = sharedKey;
    }

    public string LoginId { get; }
    public string Email { get; }
    public string RawKey { get; }
    public string SharedKey { get; }
}
