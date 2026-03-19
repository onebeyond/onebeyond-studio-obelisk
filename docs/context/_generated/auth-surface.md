# Authentication And Authorization Surface

## Setup markers

- Authentication uses a combined scheme across identity and JWT.
- Azure AD web app authentication is enabled.
- Claims principal generation uses `ApplicationClaimsIdentityFactory`.
- Cookie authentication is configured explicitly for app sessions.
- JWT authentication uses `AuthUserStore` as the user store.
- JWT bearer authentication is part of the combined auth setup.
- OpenAPI is registered per API version; v1 is currently enabled.
- Web API enables authentication middleware.
- Web API enables authorization middleware.
- Web API wires application authentication via `AddApplicationAuthentication`.

## Controllers

### `AuthController`

- Route prefix: `api/Auth/v{version:apiVersion}`
- Class attributes: `ApiVersion("1.0")`, `Produces("application/json")`
- Endpoint markers: `HttpGet PasswordRequirements`, `HttpGet Ping`, `HttpGet WhoAmI`, `HttpPost Basic/SignIn`, `HttpPost Basic/SignInWithRecoveryCode`, `HttpPost Basic/SignInWithTwoFA`, `HttpPost ChangePassword`, `HttpPost ForgotPassword`, `HttpPost ResetPassword`, `HttpPost SignOut`

### `JWTAuthenticationController`

- Route prefix: `api/account/jwt`
- Class attributes: `ApiVersionNeutral`, `Produces("application/json")`, `Route("api/account/jwt")`
- Endpoint markers: `HttpPost signIn`, `HttpPost signout`, `HttpPut refreshToken`

### `TFAController`

- Route prefix: `api/TFA/v{version:apiVersion}`
- Class attributes: `ApiVersion("1.0")`, `Authorize`, `Produces("application/json")`
- Endpoint markers: `HttpGet tfaSettings`, `HttpPost disableTfa`, `HttpPost enableTfa`, `HttpPost forgetBrowser`, `HttpPost generateRecoveryCodes`, `HttpPost generateTfaKey`, `HttpPost resetTfa`

### `UsersController`

- Route prefix: `api/Users/v{version:apiVersion}`
- Class attributes: `ApiVersion("1.0")`, `Authorize(Roles = UserRole.ADMINISTRATOR)`, `Produces("application/json")`
- Endpoint markers: `HttpPost /`, `HttpPut {loginId}/ResetPassword`, `HttpPut {userId}`, `HttpPut {userId}/Unlock`

