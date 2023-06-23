using System.Runtime.Serialization;

namespace OneBeyond.Studio.FeaturePermissions;
public class FeaturePermissionException : Exception
{
    public FeaturePermissionException(string message) : base(message)
    {
    }

    public FeaturePermissionException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public FeaturePermissionException()
    {
    }

    protected FeaturePermissionException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
