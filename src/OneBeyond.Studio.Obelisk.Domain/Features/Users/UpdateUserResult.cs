using System.Collections.Generic;
using EnsureThat;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain;

public sealed record UpdateUserResult
{
    public UpdateUserResult(bool success, IEnumerable<string> errors)
    {
        EnsureArg.IsNotNull(errors, nameof(errors));

        Success = success;
        Errors = errors;
    }

    public IEnumerable<string> Errors { get; }

    public bool Success { get; }

}
