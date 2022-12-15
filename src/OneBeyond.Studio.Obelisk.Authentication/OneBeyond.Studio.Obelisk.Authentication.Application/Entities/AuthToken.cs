using System;
using EnsureThat;
using OneBeyond.Studio.Domain.SharedKernel.Entities;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.Entities;

//It is public because we need it to be visible in Infrastructure project, where we configure entities
public sealed class AuthToken : DomainEntity<int>
{
    internal AuthToken(
        string loginId,
        string refreshToken,
        DateTimeOffset expiresOn)
        : base()
    {
        EnsureArg.IsNotNullOrWhiteSpace(loginId, nameof(loginId));
        EnsureArg.IsNotNullOrWhiteSpace(refreshToken, nameof(refreshToken));
        EnsureArg.IsNotDefault(expiresOn, nameof(expiresOn));

        LoginId = loginId;
        RefreshToken = refreshToken;
        ExpiresOn = expiresOn;
    }

#nullable disable        
    private AuthToken()
    {
    }
#nullable enable        

    public string LoginId { get; private set; }

    public string RefreshToken { get; private set; }
    public DateTimeOffset ExpiresOn { get; private set; }

    public bool IsExpired
        => ExpiresOn <= DateTimeOffset.UtcNow;
}
