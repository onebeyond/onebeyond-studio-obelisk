using EnsureThat;

namespace OneBeyond.Studio.Obelisk.WebApi.Controllers;

public sealed partial class JWTAuthenticationController
{
    public sealed record SignInJwtDto
    {
        public SignInJwtDto(
            string username,
            string password)
        {
            EnsureArg.IsNotNullOrWhiteSpace(username, nameof(username));
            EnsureArg.IsNotNullOrWhiteSpace(password, nameof(password));

            Username = username;
            Password = password;
        }

        public string Username { get; private init; }
        public string Password { get; private init; }
    }

}
