namespace OneBeyond.Studio.FeaturePermissions.AmbientContexts;
internal class AmbientStateUserIdProvider : IUserIdSetter, IUserIdAccessor
{
    private Guid? _userId = null;

    public Guid? GetLoginId()
    {
        return _userId;
    }
    public void SetUserid(Guid userId)
    {
        _userId = userId;
    }
}
