# Configuration And Environments

Use `docs/context/_generated/config-surface.md` for the current key inventory.

## Main config sources

- `appsettings.json`: base application settings
- `appsettings.Development.json`: local developer overrides
- `appsettings.QA.json`: environment-specific overrides where present
- `host.json` and `local.settings.json`: Azure Functions worker settings

## Important configuration areas

- `ConnectionStrings:ApplicationConnectionString`: database connection
- `ClientApplication:Url`: base UI URL used to generate password links
- `DomainEvents:Queue:*`: queue settings for raised domain events
- `EmailSender:*`: folder sender in development, SendGrid outside development
- `FileStorage:*`: filesystem in development, Azure Blob Storage otherwise
- `Jwt:*`: issuer, secret, and token lifetimes
- `AzureAd:*`: Azure AD settings
- `KeyVault:*`: production secret loading toggle and vault name
- `SecurityHeaders:*`: response header hardening
- `Localization:*`: supported cultures

## Environment behavior

- Development uses folder email and filesystem file storage by default.
- Non-development enables HTTPS redirection and secure antiforgery cookies.
- Key Vault can be layered into configuration through `AddKeyVault("KeyVault")`.
- The AppHost + Azurite setup is the expected local development shape.

## Secrets guidance

- Treat connection strings, JWT secrets, queue connection strings, SendGrid keys, and seeded admin passwords as secrets.
- Do not commit production secrets to repo config files.
- Prefer environment-specific secret sources such as Key Vault and deployment-time injection.

## When to update this doc

Refresh generated facts and revisit this guide when:

- a new top-level configuration area is introduced
- startup begins reading a new options section
- local-vs-non-local behavior changes
