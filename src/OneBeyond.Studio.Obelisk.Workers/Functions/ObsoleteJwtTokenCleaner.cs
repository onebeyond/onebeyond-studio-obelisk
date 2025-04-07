using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using OneBeyond.Studio.Crosscuts.Logging;
using OneBeyond.Studio.Obelisk.Authentication.Application.Services.JwtAuthentication;

namespace OneBeyond.Studio.Obelisk.Workers.Functions;

public class ObsoleteJwtTokenCleaner
{
    private readonly IJwtCleardownService _jwtCleardownService;

    private const string Schedule = "Jwt_Schedule";
    private static readonly ILogger _logger = LogManager.CreateLogger<DomainEventProcessor>();

    public ObsoleteJwtTokenCleaner(IJwtCleardownService jwtCleardownService)
    {
        EnsureArg.IsNotNull(jwtCleardownService, nameof(jwtCleardownService));
        _jwtCleardownService = jwtCleardownService;
    }

    [Function(nameof(ObsoleteJwtTokenCleaner))]
    public async Task RunAsync([TimerTrigger($"%{Schedule}%")] TimerInfo _, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Clearing down expired JWT");

        await _jwtCleardownService.ClearDownJwtTokensAsync(cancellationToken);

        _logger.LogInformation("Clearing down completed");

    }
}
