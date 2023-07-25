using System;
using EnsureThat;
using Microsoft.Extensions.Options;
using OneBeyond.Studio.Crosscuts.Strings;
using OneBeyond.Studio.Obelisk.Application.Exceptions;

namespace OneBeyond.Studio.Obelisk.WebApi.Helpers;

public sealed class ClientApplicationLinkGenerator
{
    private readonly ClientApplicationOptions _clientApplicationOptions;

    public ClientApplicationLinkGenerator(
        IOptions<ClientApplicationOptions> clientApplicationOptions) 
    {
        EnsureArg.IsNotNull(clientApplicationOptions, nameof(clientApplicationOptions));

        if (clientApplicationOptions.Value.Url.IsNullOrWhiteSpace())
        {
            throw new ObeliskApplicationException("Client application URL has not been provided"); 
        }

        _clientApplicationOptions = clientApplicationOptions.Value;
    }

    public Uri GetSetPasswordUrl(
        string loginId, 
        string resetPasswordToken)
    {
        EnsureArg.IsNotNullOrWhiteSpace(loginId, nameof(loginId));
        EnsureArg.IsNotNullOrWhiteSpace(resetPasswordToken, nameof(resetPasswordToken));

        var url = $"{_clientApplicationOptions.SetPasswordUrl}?loginId={Uri.EscapeDataString(loginId)}&code={Uri.EscapeDataString(resetPasswordToken)}";

        return new Uri(url);
    }

    public Uri GetResetPasswordUrl(
        string loginId, 
        string resetPasswordToken)
    {
        EnsureArg.IsNotNullOrWhiteSpace(resetPasswordToken, nameof(resetPasswordToken));

        var url = $"{_clientApplicationOptions.ResetPasswordUrl}?loginId={Uri.EscapeDataString(loginId)}&code={Uri.EscapeDataString(resetPasswordToken)}";

        return new Uri(url);
    }

}
