using OneBeyond.Studio.Obelisk.Authentication.Domain;
using OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Commands;

public sealed class GenerateTfaKey : LoginRequest<TfaKey>
{
    public GenerateTfaKey(string loginId)
        : base(loginId)
    {
    }
}
