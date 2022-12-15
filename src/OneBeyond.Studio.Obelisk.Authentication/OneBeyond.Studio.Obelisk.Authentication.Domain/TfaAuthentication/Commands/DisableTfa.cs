using MediatR;
using OneBeyond.Studio.Obelisk.Authentication.Domain;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Commands;

public sealed class DisableTfa : LoginRequest<Unit>
{
    public DisableTfa(
        string loginId,
        bool disableAuthenticator)
        : base(loginId)
    {
        DisableAuthenticator = disableAuthenticator;
    }

    public bool DisableAuthenticator { get; }
}
