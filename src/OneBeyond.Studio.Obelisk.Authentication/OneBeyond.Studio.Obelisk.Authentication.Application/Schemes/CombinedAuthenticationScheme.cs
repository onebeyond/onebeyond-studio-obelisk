using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.Schemes;

public sealed class CombinedAuthenticationScheme<TScheme1, TScheme2> : AuthenticationHandler<AuthenticationSchemeOptions>
    where TScheme1 : IAuthenticationScheme, new()
    where TScheme2 : IAuthenticationScheme, new()
{
    private readonly IAuthenticationScheme _primaryScheme;
    private readonly IAuthenticationScheme _secondaryScheme;

    public CombinedAuthenticationScheme(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder) : base(options, logger, encoder)
    {
        _primaryScheme = new TScheme1();
        _secondaryScheme = new TScheme2();
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authenticationResult = await Context.AuthenticateAsync(_primaryScheme.SchemeName);

        if (!authenticationResult.Succeeded)
        {
            authenticationResult = await Context.AuthenticateAsync(_secondaryScheme.SchemeName);
        }

        if (!authenticationResult.Succeeded)
        {
            Logger.LogInformation("Authentication failed: unable to authenticate user via scheme {@SchemeName1} or {@SchemeName2}", _primaryScheme.SchemeName, _secondaryScheme.SchemeName);

            return AuthenticateResult.Fail("Authentication Failed");
        }
        else
        {
            Logger.LogInformation("Authenticated successfully via scheme {@SchemeName}", authenticationResult.Ticket.AuthenticationScheme);

            return AuthenticateResult.Success(authenticationResult!.Ticket);
        }
    }
}
