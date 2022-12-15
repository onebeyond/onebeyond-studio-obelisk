using System;
using System.Runtime.Serialization;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.JwtAuthentication.Exceptions;

[Serializable]
public sealed class JwtAuthenticationFailedException : JwtException
{
    public JwtAuthenticationFailedException()
        : base()
    {
    }

    public JwtAuthenticationFailedException(string message)
        : base(message)
    {
    }

    public JwtAuthenticationFailedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    private JwtAuthenticationFailedException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

}
