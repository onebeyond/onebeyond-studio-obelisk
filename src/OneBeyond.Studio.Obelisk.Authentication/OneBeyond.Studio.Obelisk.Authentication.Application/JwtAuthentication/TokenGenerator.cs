using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EnsureThat;
using Microsoft.IdentityModel.Tokens;
using OneBeyond.Studio.Obelisk.Authentication.Application.Entities;
using OneBeyond.Studio.Obelisk.Authentication.Domain.JwtAuthentication;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.JwtAuthentication;

internal static class TokenGenerator
{
    private const int DEFAULT_ACCESS_TOKEN_EXPIRATION_MINUTES = 30;
    private const int DEFAULT_REFRESH_TOKEN_EXPIRATION_DAYS = 7;

    internal static string GenerateJwtToken(
        AuthUser user,
        IEnumerable<Claim> additionalClaims,
        JwtAuthenticationOptions jwtConfiguration)
    {
        EnsureArg.IsNotNull(user, nameof(user));
        EnsureArg.IsNotNull(additionalClaims, nameof(additionalClaims));
        EnsureArg.IsNotNull(jwtConfiguration, nameof(jwtConfiguration));

        jwtConfiguration.EnsureIsValid();

        var tokenHandler = new JwtSecurityTokenHandler();

        var key = Encoding.UTF8.GetBytes(jwtConfiguration.Secret!);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = jwtConfiguration.Issuer,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),

            Expires = DateTime.UtcNow.AddMinutes(jwtConfiguration.AccessTokenExpirationMinutes ?? DEFAULT_ACCESS_TOKEN_EXPIRATION_MINUTES),

            Claims = new Dictionary<string, object?>
            {
                { ClaimTypes.NameIdentifier, user.UserName },
                { JwtClaims.JWT_CLAIM_LOGINID, user.Id }
            }
        };

        foreach (var claim in additionalClaims)
        {
            tokenDescriptor.Claims.Add(claim.Type, claim.Value);
        }

        var issuedToken = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(issuedToken);
    }

    internal static (string token, DateTimeOffset expiryDate) GenerateRefreshToken(JwtAuthenticationOptions jwtConfiguration)
    {
        EnsureArg.IsNotNull(jwtConfiguration, nameof(jwtConfiguration));

        return (
            Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            DateTimeOffset.UtcNow.AddDays(jwtConfiguration.RefreshTokenExpirationDays ?? DEFAULT_REFRESH_TOKEN_EXPIRATION_DAYS));
    }

}
