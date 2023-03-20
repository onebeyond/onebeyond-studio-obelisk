using MediatR;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Commands;

public sealed record DisableTfa : LoginRequest<Unit>
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
