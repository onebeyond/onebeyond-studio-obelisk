using System.Threading;
using System.Threading.Tasks;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.AuthenticationFlows;

public interface IAuthenticationFlowHandler
{
    /// <summary>
    /// Allows to do application specific post processing against the login,
    /// such as putting a record into application' login log
    /// </summary>
    /// <param name="loginId"></param>
    /// <param name="signInStatus"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task OnSignInCompletedAsync(string loginId, SignInStatus signInStatus, CancellationToken cancellationToken);

    /// <summary>
    /// Allows to do application specific validations against the login.
    /// This can be called before actual signing in gets completed.
    /// Throwing an exception of any kind will reject the login and prevent a user
    /// from being authenticated
    /// </summary>
    /// <param name="loginId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task OnValidatingLoginAsync(string loginId, CancellationToken cancellationToken);

    /// <summary>
    /// Allows a domain to specify on per user bases which users require two factor authentication 
    /// </summary>
    /// <param name="loginId"></param>
    /// <returns>True, if two factor authentication if required for a user</returns>
    Task<bool> IsTwoFactorAthenticationRequiredForLoginAsync(string loginId, CancellationToken cancellationToken);
}
