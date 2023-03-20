namespace OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Queries;

public sealed record GetTfaSettings : LoginRequest<LoginTfaSettings>
{
    public GetTfaSettings(string loginId)
        : base(loginId)
    {
    }
}
