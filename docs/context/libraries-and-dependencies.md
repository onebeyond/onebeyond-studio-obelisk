# Libraries And Dependencies

Use `docs/context/_generated/dependency-inventory.md` for the exact current package inventory.

## Dependency management model

- Central package versions are defined in `Directory.Packages.props`.
- Common project settings are defined in `Directory.Build.props`.
- Most runtime projects consume shared One Beyond packages plus ASP.NET Core, EF Core, and Azure-related packages.

## Main library families

- `OneBeyond.Studio.*`: shared platform libraries for mediator, data access, hosting, infrastructure, file storage, email, and template rendering
- `Autofac.*`: container integration
- `Asp.Versioning.*`: API versioning and explorer support
- `Microsoft.AspNetCore.OpenApi` and Swagger UI: OpenAPI generation and UI
- `Microsoft.AspNetCore.Identity.*`: identity storage and auth flows
- `Microsoft.Identity.Web`: Azure AD integration
- `Microsoft.Azure.Functions.Worker*`: background worker runtime
- `Aspire.Hosting.Azure.Functions`: local orchestration support
- `OpenTelemetry.*` and Application Insights packages: telemetry

## Working conventions

- Prefer adding new package versions to `Directory.Packages.props` instead of pinning versions per project.
- Before adding a dependency, check whether the capability already exists in `OneBeyond.Studio.*`.
- Treat new infrastructure packages as review-heavy changes because they often affect hosting, configuration, or deployment.

## What to review carefully

- auth packages
- EF Core packages and migrations tooling
- storage, queue, or cloud SDK changes
- packages that change startup, middleware, or OpenAPI behavior
