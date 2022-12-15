using System;
using System.Runtime.Serialization;
using OneBeyond.Studio.Crosscuts.Exceptions;

namespace OneBeyond.Studio.Obelisk.Application.Exceptions;

[Serializable]
public class ObeliskApplicationException : OneBeyondException
{
    public ObeliskApplicationException()
    {
    }

    public ObeliskApplicationException(string message)
        : base(message)
    {
    }

    public ObeliskApplicationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    protected ObeliskApplicationException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
