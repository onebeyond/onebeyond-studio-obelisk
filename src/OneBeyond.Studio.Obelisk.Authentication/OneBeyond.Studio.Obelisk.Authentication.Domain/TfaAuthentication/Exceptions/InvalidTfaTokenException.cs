using System;
using System.Runtime.Serialization;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Exceptions;

[Serializable]
public sealed class InvalidTfaTokenException : TfaException
{
    public InvalidTfaTokenException()
        : base()
    {
    }

    public InvalidTfaTokenException(string message)
        : base(message)
    {
    }

    public InvalidTfaTokenException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    private InvalidTfaTokenException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

}
