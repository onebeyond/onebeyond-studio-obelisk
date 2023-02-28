using System.Collections.Generic;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Commands;

public sealed record GenerateTfaRecoveryCodes : LoginRequest<IEnumerable<string>>
{
    public GenerateTfaRecoveryCodes(string loginId)
        : base(loginId)
    {
    }
}
