using System;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.Exceptions;

[Serializable]
public class AuthException : Exception
{
    public AuthException()
        : base()
    {
    }

    public AuthException(string message)
        : base(message)
    {
    }

    public AuthException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
