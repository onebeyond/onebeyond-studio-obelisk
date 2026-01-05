namespace OneBeyond.Studio.Obelisk.Authentication.Domain.TfaAuthentication.Commands;

public sealed record GenerateTfaKey : LoginCommand<TfaKey>
{
    public GenerateTfaKey(string loginId)
        : base(loginId)
    {
    }
}
