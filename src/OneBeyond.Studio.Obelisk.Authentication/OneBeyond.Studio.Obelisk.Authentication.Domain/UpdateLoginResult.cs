using System.Collections.Generic;
using EnsureThat;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain;

public sealed record UpdateLoginResult
{
    public UpdateLoginResult(bool success, IEnumerable<string> errors)
    {
        EnsureArg.IsNotNull(errors, nameof(errors));

        Success = success;
        Errors = errors;
    }

    public IEnumerable<string> Errors { get; }

    public bool Success { get; }

}
