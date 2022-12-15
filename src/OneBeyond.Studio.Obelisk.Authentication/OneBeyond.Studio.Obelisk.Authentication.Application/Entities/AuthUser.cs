using System;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.Entities;

public class AuthUser : Microsoft.AspNetCore.Identity.IdentityUser<string>
{
    private readonly List<AuthToken> _authTokens = new List<AuthToken>();

    public DateTime? LastLogin { get; set; }
    public string? PasswordResetKey { get; set; }

    public IReadOnlyCollection<AuthToken> AuthTokens => _authTokens.AsReadOnly();

    internal AuthToken AddAuthToken(
        string refreshToken,
        DateTimeOffset expiresOn)
    {
        EnsureArg.IsNotNullOrWhiteSpace(refreshToken, nameof(refreshToken));
        EnsureArg.IsNotDefault(expiresOn, nameof(expiresOn));

        var token = new AuthToken(Id, refreshToken, expiresOn);

        _authTokens.Add(token);

        return token;
    }

    internal void ExpireToken(string refreshToken)
    {
        EnsureArg.IsNotNullOrWhiteSpace(refreshToken, nameof(refreshToken));

        var token = _authTokens.FirstOrDefault(x => x.RefreshToken == refreshToken);

        if (token != null)
        {
            _authTokens.Remove(token);
        }
    }

}
