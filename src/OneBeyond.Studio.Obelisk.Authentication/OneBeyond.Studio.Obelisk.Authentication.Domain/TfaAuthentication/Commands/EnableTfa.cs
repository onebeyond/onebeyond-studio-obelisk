using EnsureThat;
using MediatR;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Commands;

public sealed record EnableTfa : LoginRequest<Unit>
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
