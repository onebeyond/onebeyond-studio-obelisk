using EnsureThat;
using MediatR;
using OneBeyond.Studio.Obelisk.Authentication.Domain;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Commands;

public sealed class EnableTfa : LoginRequest<Unit>
{
    public EnableTfa(
        string loginId,
        string verificationCode)
        : base(loginId)
    {
        EnsureArg.IsNotNullOrWhiteSpace(verificationCode, nameof(verificationCode));

        VerificationCode = verificationCode;
    }

    public string VerificationCode { get; }
}
