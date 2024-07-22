using System;
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
}
