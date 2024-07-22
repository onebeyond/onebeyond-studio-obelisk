using System;
using OneBeyond.Studio.Crosscuts.Exceptions;

namespace OneBeyond.Studio.Obelisk.Infrastructure.Exceptions;

[Serializable]
public class ObeliskInfrastructureException : OneBeyondException
{
    public ObeliskInfrastructureException()
    {
    }

    public ObeliskInfrastructureException(string message)
        : base(message)
    {
    }

    public ObeliskInfrastructureException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
