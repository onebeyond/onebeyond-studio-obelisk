namespace OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Queries;

public sealed class GetTfaSettings : LoginRequest<LoginTfaSettings>
{
    public GetTfaSettings(string loginId)
        : base(loginId)
    {
    }
}
