using System;
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
}
