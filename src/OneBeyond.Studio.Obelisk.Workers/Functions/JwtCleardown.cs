using System;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using OneBeyond.Studio.Crosscuts.Logging;
using OneBeyond.Studio.Obelisk.Authentication.Application.Services.JwtAuthentication;

namespace OneBeyond.Studio.Obelisk.Workers.Functions;

public class JwtCleardown
{
    private readonly IJwtTokenService _jwtTokenService;

    private const string Schedule = "Jwt_Schedule";
    private static readonly ILogger _logger = LogManager.CreateLogger<DomainEventProcessor>();

    public JwtCleardown(IJwtTokenService jwtTokenService)
    {
        EnsureArg.IsNotNull(jwtTokenService, nameof(jwtTokenService));
        _jwtTokenService = jwtTokenService;
    }

    [Function(nameof(JwtCleardown))]
    public async Task RunAsync([TimerTrigger($"%{Schedule}%")] TimerInfo _, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Clearing down expired JWT");

        await _jwtTokenService.CleardownExpiredTokensAsync(cancellationToken);
        
    }
}
