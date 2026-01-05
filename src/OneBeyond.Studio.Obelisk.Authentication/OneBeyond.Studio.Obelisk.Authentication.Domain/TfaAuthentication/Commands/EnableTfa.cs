using System.Collections.Generic;
using EnsureThat;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Commands;

public sealed record EnableTfa : LoginCommand<IEnumerable<string>>
{
    public EnableTfa(
        string loginId,
        string verificationCode)
        : base(loginId)
    {
        EnsureArg.IsNotNullOrWhiteSpace(verificationCode, nameof(verificationCode));

        VerificationCode = verificationCode.Replace(" ", string.Empty).Replace("-", string.Empty);
    }

    public string VerificationCode { get; }
}
