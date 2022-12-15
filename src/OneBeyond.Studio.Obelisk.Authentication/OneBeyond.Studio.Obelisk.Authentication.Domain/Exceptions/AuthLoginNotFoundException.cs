using System;
using System.Runtime.Serialization;

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

    private AuthLoginNotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

}
