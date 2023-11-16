using System;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.Exceptions;

[Serializable]
public sealed class AuthLoginNotFoundException : AuthException
{
    public AuthLoginNotFoundException()
        : base()
    {
    }

    public AuthLoginNotFoundException(string message)
        : base(message)
    {
    }

    public AuthLoginNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
