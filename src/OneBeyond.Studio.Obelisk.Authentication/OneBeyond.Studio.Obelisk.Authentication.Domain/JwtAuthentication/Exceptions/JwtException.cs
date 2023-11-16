using System;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Exceptions;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.JwtAuthentication.Exceptions;

[Serializable]
public class JwtException : AuthException
{
    public JwtException()
        : base()
    {
    }

    public JwtException(string message)
        : base(message)
    {
    }

    public JwtException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
