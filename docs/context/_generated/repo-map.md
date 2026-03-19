# Repository Map

Generated from `onebeyond-studio-obelisk`.

## Top-level layout

- `.github/`: CI and repository automation
- `devops/`: deployment and infrastructure support assets
- `src/`: runtime and test projects
- `docs/context/`: Context Pack entrypoint, guidance, and generated fact sheets
- `tools/Obelisk.ContextPack/`: generator used to refresh `docs/context/_generated`

## Solution projects

- `src/OneBeyond.Studio.Obelisk.AppHost/OneBeyond.Studio.Obelisk.AppHost.csproj`: `net10.0`; role `Aspire orchestration`
- `src/OneBeyond.Studio.Obelisk.Application/OneBeyond.Studio.Obelisk.Application.csproj`: `net10.0`; role `Application`
- `src/OneBeyond.Studio.Obelisk.Authentication/OneBeyond.Studio.Obelisk.Authentication.Application/OneBeyond.Studio.Obelisk.Authentication.Application.csproj`: `net10.0`; role `Application`
- `src/OneBeyond.Studio.Obelisk.Authentication/OneBeyond.Studio.Obelisk.Authentication.Domain/OneBeyond.Studio.Obelisk.Authentication.Domain.csproj`: `net10.0`; role `Domain`
- `src/OneBeyond.Studio.Obelisk.Domain.Tests/OneBeyond.Studio.Obelisk.Domain.Tests.csproj`: `net10.0`; role `Domain`; test project
- `src/OneBeyond.Studio.Obelisk.Domain/OneBeyond.Studio.Obelisk.Domain.csproj`: `net10.0`; role `Domain`
- `src/OneBeyond.Studio.Obelisk.Infrastructure/OneBeyond.Studio.Obelisk.Infrastructure.csproj`: `net10.0`; role `Infrastructure`
- `src/OneBeyond.Studio.Obelisk.ServiceDefaults/OneBeyond.Studio.Obelisk.ServiceDefaults.csproj`: `net10.0`; role `Aspire service defaults`
- `src/OneBeyond.Studio.Obelisk.WebApi.Tests/OneBeyond.Studio.Obelisk.WebApi.Tests.csproj`: `net10.0`; role `HTTP API`; test project
- `src/OneBeyond.Studio.Obelisk.WebApi/OneBeyond.Studio.Obelisk.WebApi.csproj`: `net10.0`; role `HTTP API`
- `src/OneBeyond.Studio.Obelisk.Workers/OneBeyond.Studio.Obelisk.Workers.csproj`: `net10.0`; role `Azure Functions worker`

## Recommended editing map

- Domain commands, entities, and domain events live under `src/OneBeyond.Studio.Obelisk.Domain`.
- Query DTOs and handlers live under `src/OneBeyond.Studio.Obelisk.Application`.
- Identity and auth primitives live under `src/OneBeyond.Studio.Obelisk.Authentication`.
- EF Core data access, seeding, and migrations live under `src/OneBeyond.Studio.Obelisk.Infrastructure`.
- HTTP endpoints live under `src/OneBeyond.Studio.Obelisk.WebApi`.
- Azure Functions background processing lives under `src/OneBeyond.Studio.Obelisk.Workers`.
- Aspire orchestration lives under `src/OneBeyond.Studio.Obelisk.AppHost`.
