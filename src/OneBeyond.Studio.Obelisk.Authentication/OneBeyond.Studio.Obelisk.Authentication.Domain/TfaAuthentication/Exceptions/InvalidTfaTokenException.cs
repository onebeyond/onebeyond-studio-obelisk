using System;

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
}
