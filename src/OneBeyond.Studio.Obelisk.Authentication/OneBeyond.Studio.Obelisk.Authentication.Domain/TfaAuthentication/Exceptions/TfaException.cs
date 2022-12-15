using System;
using System.Runtime.Serialization;
using OneBeyond.Studio.Obelisk.Authentication.Domain.Exceptions;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Exceptions;

[Serializable]
public class TfaException : AuthException
{
    public TfaException()
        : base()
    {
    }

    public TfaException(string message)
        : base(message)
    {
    }

    public TfaException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    protected TfaException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
