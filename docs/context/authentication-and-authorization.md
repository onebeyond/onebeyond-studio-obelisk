# Authentication And Authorization

Use `docs/context/_generated/auth-surface.md` for the current controller and startup auth surface.

## Authentication modes in this template

The template combines multiple auth paths:

- application cookie auth for browser/session flows
- JWT bearer auth for token-based API use
- Azure AD web app auth through Microsoft Identity Web

The Web API startup wires this through `AddApplicationAuthentication(...)`, then adds:

- `AuthUserStore` for JWT-related user storage
- `ApplicationClaimsIdentityFactory` for claims principal generation
- a combined auth scheme across identity and JWT

## Cookie behavior

- cookie auth is configured explicitly after `AddIdentity`
- cross-site cookies are controlled by `CookieAuthN:AllowCrossSiteCookies`
- security stamp validation is tightened to invalidate logout sessions quickly

## JWT behavior

- JWT auth is configured from the `Jwt` settings section
- the `JWTAuthenticationController` exposes sign-in, refresh-token, and sign-out-all-tokens endpoints
- background JWT cleanup services run in the worker host

## Azure AD behavior

- Azure AD is enabled via `AddMicrosoftIdentityWebApp`
- settings come from the `AzureAd` section

## Authorization conventions

- controller-level `[Authorize]` is the default mechanism
- role-restricted endpoints use `[Authorize(Roles = ...)]`
- `UsersController` is restricted to administrators
- auth endpoints mix anonymous and authorized actions depending on the flow

## What to review carefully

- changes to auth startup wiring
- changes to cookie policy and cross-site behavior
- changes to combined scheme behavior
- changes to `[Authorize]` attributes or role restrictions
- changes to password-reset, sign-in, and sign-out flows
