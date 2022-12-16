using System;
using System.Runtime.Serialization;

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

    protected AuthException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
