using System;
using EnsureThat;
using OneBeyond.Studio.Domain.SharedKernel.Entities;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.Entities;

//It is public because we need it to be visible in Infrastructure project, where we configure entities
public sealed class AuthToken : AggregateRoot<int>
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
        Refreshed = false;
    }

#nullable disable        
    private AuthToken()
    {
    }
#nullable enable        

    public string LoginId { get; private set; }

    public string RefreshToken { get; private set; }
    public DateTimeOffset ExpiresOn { get; private set; }
    public bool Refreshed { get; private set; }

    public void Expire()
    {
        Refreshed = true;
        ExpiresOn = DateTimeOffset.UtcNow;
    }

    public bool IsExpired
        => ExpiresOn <= DateTimeOffset.UtcNow;
}
