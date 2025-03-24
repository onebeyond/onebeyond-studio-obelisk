using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using OneBeyond.Studio.Application.SharedKernel.Repositories;
using OneBeyond.Studio.Crosscuts.Logging;
using OneBeyond.Studio.Obelisk.Authentication.Application.Entities;
using OneBeyond.Studio.Obelisk.Authentication.Application.JwtAuthentication;
using OneBeyond.Studio.Obelisk.Authentication.Application.Services.ApplicationClaims;
using OneBeyond.Studio.Obelisk.Authentication.Domain.JwtAuthentication;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.Services.JwtAuthentication;

internal sealed class JwtTokenService : IJwtTokenService
{
    private readonly UserManager<AuthUser> _userManager;
    private readonly JwtAuthenticationOptions _jwtConfiguration;
    private readonly IApplicationClaimsService _applicationClaimsService;
    private readonly IRWRepository<AuthToken, int> _rWRepository;
    private readonly IRORepository<AuthToken, int> _roRepository;

    private static readonly ILogger _logger = LogManager.CreateLogger<JwtTokenService>();

    public JwtTokenService(
        UserManager<AuthUser> userManager,
        JwtAuthenticationOptions jwtConfiguration,
        IApplicationClaimsService applicationClaimsService,
        IRWRepository<AuthToken, int> rWRepository,
        IRORepository<AuthToken, int> roRepository)
    {
        EnsureArg.IsNotNull(userManager, nameof(userManager));
        EnsureArg.IsNotNull(jwtConfiguration, nameof(jwtConfiguration));
        EnsureArg.IsNotNull(applicationClaimsService, nameof(applicationClaimsService));
        EnsureArg.IsNotNull(rWRepository, nameof(rWRepository));
        EnsureArg.IsNotNull(roRepository, nameof(roRepository));

        _userManager = userManager;
        _jwtConfiguration = jwtConfiguration;
        _applicationClaimsService = applicationClaimsService;
        _rWRepository = rWRepository;
        _roRepository = roRepository;
    }

    public Task<JwtToken> RefreshTokenAsync(
        AuthUser identityUser,
        string oldRefreshToken,
        CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(identityUser, nameof(identityUser));
        EnsureArg.IsNotNullOrWhiteSpace(oldRefreshToken, nameof(oldRefreshToken));

        identityUser.ExpireToken(oldRefreshToken);

        return CreateTokenAsync(
                identityUser,
                cancellationToken);
    }

    public async Task<JwtToken> CreateTokenAsync(
        AuthUser identityUser,
        CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(identityUser, nameof(identityUser));

        var userRoles = (await _userManager.GetRolesAsync(identityUser).ConfigureAwait(false))
            .Select(role => new Claim(ClaimTypes.Role, role))
            .ToList();

        var applicationClaims = await _applicationClaimsService.ListApplicationClaimsForUserAsync(
                identityUser.Id,
                cancellationToken)
            .ConfigureAwait(false);

        var additionalClaims = userRoles.Concat(applicationClaims);

        try
        {
            var jwtToken = TokenGenerator.GenerateJwtToken(
                identityUser,
                additionalClaims,
                _jwtConfiguration);

            var (refreshToken, expiresOn) = TokenGenerator.GenerateRefreshToken(_jwtConfiguration);

            identityUser.AddAuthToken(refreshToken, expiresOn);

            await _userManager.UpdateAsync(identityUser).ConfigureAwait(false); //Save auth token

            return new JwtToken(identityUser.Id, jwtToken, refreshToken);
        }
        catch (Exception exc)
        {
            // Rethrow as the UI handles this fine - but this exception needs logging as it usually
            // indicates something wrong with the setup
            _logger.LogError(exc, "Error generating JWT for user {userId}", identityUser.Id);
            throw;
        }        
    }

    public async Task SignOutAsync(AuthUser identityUser)
    {
        EnsureArg.IsNotNull(identityUser, nameof (identityUser));
        identityUser.SignOutAllTokens();
        await _userManager.UpdateAsync(identityUser);
    }

    public async Task CleardownExpiredTokensAsync(CancellationToken cancellationToken) 
    {
        _logger.LogInformation("Clearing down expired JWT");
        var expiredTokens = await _roRepository
            .ListAsync(x => x.ExpiresOn < DateTimeOffset.UtcNow.AddDays(-1), 
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        _logger.LogInformation("{tokenCount} tokens to clear", expiredTokens.Count);
        
        foreach (var expiredToken in expiredTokens)
        {
            await _rWRepository.DeleteAsync(expiredToken.Id, cancellationToken).ConfigureAwait(false);
        }

        _logger.LogInformation("Clear down complete");
    }
}
