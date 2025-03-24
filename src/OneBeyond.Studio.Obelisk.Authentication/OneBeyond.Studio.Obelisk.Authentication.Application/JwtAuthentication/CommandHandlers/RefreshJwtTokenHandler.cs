using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OneBeyond.Studio.Application.SharedKernel.Repositories;
using OneBeyond.Studio.Obelisk.Authentication.Application.Entities;
using OneBeyond.Studio.Obelisk.Authentication.Application.Services.JwtAuthentication;
using OneBeyond.Studio.Obelisk.Authentication.Domain.JwtAuthentication;
using OneBeyond.Studio.Obelisk.Authentication.Domain.JwtAuthentication.Commands;
using OneBeyond.Studio.Obelisk.Authentication.Domain.JwtAuthentication.Exceptions;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.JwtAuthentication.CommandHandlers;

internal sealed class RefreshJwtTokenHandler : IRequestHandler<RefreshJwtToken, JwtToken>
{
    private readonly UserManager<AuthUser> _userManager;
    private readonly IRORepository<AuthToken, int> _authTokenRORepository;
    private readonly IJwtTokenService _jWTokenService;

    public RefreshJwtTokenHandler(
        UserManager<AuthUser> userManager,
        IRORepository<AuthToken, int> authTokenRORepository,
        IJwtTokenService jWTokenService)
    {
        EnsureArg.IsNotNull(userManager, nameof(userManager));
        EnsureArg.IsNotNull(authTokenRORepository, nameof(authTokenRORepository));
        EnsureArg.IsNotNull(jWTokenService, nameof(jWTokenService));

        _userManager = userManager;
        _authTokenRORepository = authTokenRORepository;
        _jWTokenService = jWTokenService;
    }

    public async Task<JwtToken> Handle(RefreshJwtToken command, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        var tokens = await _authTokenRORepository.ListAsync(x => x.RefreshToken == command.RefreshToken, cancellationToken: cancellationToken).ConfigureAwait(false);

        var token = tokens.SingleOrDefault();

        if (token == null)
        {
            throw new JwtAuthenticationFailedException("Refresh token not found.");
        }

        // Defends against replay attacks. In the event that a token has already been refreshed, everyone
        // loses all their tokens, as that's the only possible way this happens. A "normally expired" 
        // token will not be considered "Refreshed".
        if (token.Refreshed)
        {
            var user = await _userManager.FindByIdAsync(token.LoginId).ConfigureAwait(false)
                ?? throw new JwtAuthenticationFailedException($"Login with id {token.LoginId} not found.");
            await _jWTokenService.SignOutAsync(user);

            throw new JwtAuthenticationFailedException("Refresh token expired");
        }

        if (token.IsExpired)
        {
            throw new JwtAuthenticationFailedException("Refresh token expired.");
        }

        var identityUser = await _userManager.FindByIdAsync(token.LoginId).ConfigureAwait(false)
            ?? throw new JwtAuthenticationFailedException($"Login with id {token.LoginId} not found.");

        return await _jWTokenService.RefreshTokenAsync(
                identityUser,
                token.RefreshToken,
                cancellationToken)
            .ConfigureAwait(false);
    }
}
