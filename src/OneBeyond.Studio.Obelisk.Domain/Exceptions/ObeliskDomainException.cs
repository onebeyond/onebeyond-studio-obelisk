using System;
using System.Runtime.Serialization;
using OneBeyond.Studio.Crosscuts.Exceptions;

namespace OneBeyond.Studio.Obelisk.Domain.Exceptions;

[Serializable]
public class ObeliskDomainException : OneBeyondException
{
    public ObeliskDomainException()
        : base()
    {
    }

    public ObeliskDomainException(string message)
        : base(message)
    {
    }

    public ObeliskDomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    protected ObeliskDomainException(SerializationInfo info, StreamingContext context)
         : base(info, context)
    {
    }
}
