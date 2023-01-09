using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.Extensions.Logging;
using OneBeyond.Studio.Application.SharedKernel.Repositories;
using OneBeyond.Studio.Crosscuts.Logging;
using OneBeyond.Studio.Domain.SharedKernel.Entities.Dto;
using OneBeyond.Studio.Obelisk.Application.Features.Users.Dto;
using OneBeyond.Studio.Obelisk.Application.Features.Users.Predicates;
using OneBeyond.Studio.Obelisk.Authentication.Domain;
using OneBeyond.Studio.Obelisk.Authentication.Domain.AuthenticationFlows;
using OneBeyond.Studio.Obelisk.Domain.Exceptions;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Entities;

namespace OneBeyond.Studio.Obelisk.Application.Services.AuthenticationFlows;

internal sealed class AuthenticationFlowHandler : IAuthenticationFlowHandler
{
    private static readonly ILogger Logger = LogManager.CreateLogger<AuthenticationFlowHandler>();

    private readonly IRORepository<UserBase, Guid> _userRORepository;
    private readonly Lazy<IRWRepository<UserBase, Guid>> _userRWRepository;
    private readonly Lazy<IRWRepository<UserLoginLog, int>> _loginLogRWRepository;

    public AuthenticationFlowHandler(
        IRORepository<UserBase, Guid> userRORepository,
        Lazy<IRWRepository<UserBase, Guid>> userRWRepository,        // Load it lazily as it is needed for hadnling sign in only
        Lazy<IRWRepository<UserLoginLog, int>> loginLogRWRepository) // Load it lazily as it is needed for handling sign in only
    {
        EnsureArg.IsNotNull(userRORepository, nameof(userRORepository));
        EnsureArg.IsNotNull(userRWRepository, nameof(userRWRepository));
        EnsureArg.IsNotNull(loginLogRWRepository, nameof(loginLogRWRepository));

        _userRORepository = userRORepository;
        _userRWRepository = userRWRepository;
        _loginLogRWRepository = loginLogRWRepository;
    }

    public Task<bool> IsTwoFactorAthenticationRequiredForLoginAsync(string loginId, CancellationToken cancellationToken)
    {
        //Add your domain specific business logic to define if two factor authentication is required fo a user specified
        //By default we switch off TFA for all users
        return Task.FromResult(false);
    }

    public async Task OnSignInCompletedAsync(
        string loginId,
        SignInStatus signInStatus,
        CancellationToken cancellationToken)
    {
        var userDto = (await _userRORepository.ListAsync<LookupItemDto<Guid>>(
                UserPredicates.ByLoginId(loginId),
                cancellationToken: cancellationToken).ConfigureAwait(false))
            .SingleOrDefault();

        if (userDto is null)
        {
            Logger.LogError(
                "Reject signing in as not being able to find a user for login {LoginId}",
                loginId);
            throw new ObeliskDomainException("Unable to find user account for the specified login.");
        }

        if (signInStatus == SignInStatus.LockedOut)
        {
            var userRWRepository = _userRWRepository.Value;

            var user = await userRWRepository.GetByIdAsync(
                userDto.Id,
                cancellationToken).ConfigureAwait(false);

            user.Lock();

            await userRWRepository.UpdateAsync(user, cancellationToken).ConfigureAwait(false);
        }

        var loginLogRWRepository = _loginLogRWRepository.Value;

        await loginLogRWRepository.CreateAsync(
            new UserLoginLog(userDto.Name!, signInStatus == SignInStatus.Success, signInStatus.ToString()),
            cancellationToken).ConfigureAwait(false);
    }

    public async Task OnValidatingLoginAsync(
        string loginId,
        CancellationToken cancellationToken)
    {
        var user = (await _userRORepository.ListAsync<UserIsActiveDto>(
                UserPredicates.ByLoginId(loginId),
                cancellationToken: cancellationToken).ConfigureAwait(false))
            .SingleOrDefault();

        if (user is null)
        {
            Logger.LogError(
                "Reject authnetication as not being able to find a user for login {LoginId}",
                loginId);
            throw new ObeliskDomainException("Unable to find user account for the specified login.");
        }

        if (!user.IsActive)
        {
            Logger.LogInformation(
                "Reject authentication as user {UserId} is deactivated",
                user.Id);
            throw new ObeliskDomainException("User account is deactivated.");
        }
    }
}
