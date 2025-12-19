using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Identity;
using OneBeyond.Studio.Core.Mediator.Commands;
using OneBeyond.Studio.Obelisk.Authentication.Application.Entities;
using OneBeyond.Studio.Obelisk.Authentication.Application.Services.JwtAuthentication;
using OneBeyond.Studio.Obelisk.Authentication.Domain.JwtAuthentication.Commands;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.JwtAuthentication.CommandHandlers;
internal class SignoutJwtHandler : ICommandHandler<SignOutAllTokens>
{
    private readonly SignInManager<AuthUser> _signInManager;
    private readonly UserManager<AuthUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;

    public SignoutJwtHandler(SignInManager<AuthUser> signInManager, UserManager<AuthUser> userManager, IJwtTokenService jwtTokenService)
    {
        EnsureArg.IsNotNull(signInManager, nameof(signInManager));
        EnsureArg.IsNotNull(userManager, nameof(userManager));
        EnsureArg.IsNotNull(jwtTokenService, nameof(jwtTokenService));

        _signInManager = signInManager;
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
    }

    public async Task HandleAsync(SignOutAllTokens request, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(request, nameof(request));
        var identityUser = await _userManager.FindByNameAsync(request.UserName);
        await _jwtTokenService.SignOutAsync(identityUser!);
        await _signInManager.SignOutAsync();
    }
}
