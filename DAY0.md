# Day 0: Starting A New AI-Assisted Obelisk Project

This guide combines:

- the wiki's greenfield bootstrap flow for creating a new Obelisk-based project
- the repo-local Context Pack's current guidance for how the template should be run and extended today

Use the wiki for initial project creation and setup details.
Use `docs/context/` as the source of truth for architecture, layering, generated repository facts, and AI working boundaries once the project exists.

If you are using Codex skills, this workflow is also available as `$bootstrap-obelisk-greenfield`.

## 1. Create The Repositories

Create:

- one backend repository with default branch `main`
- one frontend repository if the project needs a SPA

The wiki assumes backend and frontend live in separate repositories.

## 2. Generate The Backend Solution From The Template

Get the latest backend template source and run the template generator described in the wiki.

Example:

```powershell
git clone https://github.com/onebeyond/onebeyond-studio-obelisk C:\dev\onebeyond-studio-obelisk

C:\tools\OneBeyondStudioTemplateGenerator\OneBeyondStudioTemplateGenerator.exe `
  "C:\dev\onebeyond-studio-obelisk" `
  "C:\dev\Manga Reader" `
  "MangaReader"
```

Notes:

- the project name passed to the generator should contain only letters, digits, and full stops
- the generated output becomes the starting point for your new backend repository

## 3. Push The Generated Backend Into Your New Repo

Example:

```powershell
git clone <your-backend-repo-url> C:\dev\MangaReader.Backend
Copy-Item "C:\dev\Manga Reader\*" "C:\dev\MangaReader.Backend" -Recurse -Force
git -C C:\dev\MangaReader.Backend add .
git -C C:\dev\MangaReader.Backend commit -m "Initial Obelisk setup"
git -C C:\dev\MangaReader.Backend push origin main
```

## 4. Install Local Prerequisites

Before running the backend locally, install:

- .NET SDK 10
- Docker Desktop
- Azure Functions Core Tools

## 5. Read The Context Pack First

In the new backend repository, start with:

1. `docs/context/overview.md`
2. `docs/context/architecture.md`
3. `docs/context/_generated/repo-map.md`
4. `docs/context/_generated/dependency-inventory.md`
5. `docs/context/_generated/config-surface.md`
6. `docs/context/_generated/auth-surface.md`
7. `docs/context/feature-delivery-guide.md`

This is the fastest way to align engineers and AI tools with the current template shape.

## 6. Create Project-Specific AI Context

Create the following files from the provided templates:

```powershell
Copy-Item docs\context\project-profile.template.md docs\context\project-profile.md
Copy-Item docs\context\domain-glossary.template.md docs\context\domain-glossary.md
```

Then fill them in:

- `docs/context/project-profile.md`
  - project name
  - purpose
  - primary users
  - business-critical areas
  - key integrations
  - environments
  - safe AI-assisted work
  - review-heavy work
  - engineer-led work
- `docs/context/domain-glossary.md`
  - key business terms and meanings

This helps AI produce project-specific changes instead of generic template-shaped output.

## 7. Respect The AI Working Boundaries

AI can safely lead:

- DTO creation
- straightforward endpoint plumbing
- routine command/query scaffolding
- documentation updates
- generated fact refreshes
- boilerplate tests

Human review is especially important for:

- authentication and authorization changes
- migrations and persistence changes
- queue or domain-event handling
- file storage and email flows
- deployment-sensitive configuration changes

Engineer-led work should include:

- aggregate and domain-model design
- business invariants
- security-sensitive changes
- system-boundary decisions

## 8. Configure The Baseline Application Settings

Review and set the main configuration areas early:

- `ConnectionStrings:ApplicationConnectionString`
- `ClientApplication:Url`
- `DomainEvents:Queue:*`
- `EmailSender:*`
- `FileStorage:*`
- `Jwt:*`
- `AzureAd:*`
- `KeyVault:*`

Typical places to update:

- `appsettings.json`
- `appsettings.Development.json`
- environment-specific appsettings files
- `host.json`
- `local.settings.json`

Do not commit production secrets.

## 9. Run The Backend Using The Current Template Shape

The wiki bootstrap still describes separate `WebApi` and `Workers` startup, but the current template guidance uses Aspire orchestration through `AppHost`.

For new greenfield work, prefer running via `AppHost`.

Example:

```powershell
dotnet run --project src\<YourProject>.AppHost\<YourProject>.AppHost.csproj
```

Expected local shape:

- `WebApi` for the HTTP surface
- `Workers` for background processing
- Azurite-backed storage for local development

## 10. Verify The Baseline Before Custom Feature Work

Before building project features, confirm:

- the solution starts cleanly
- the API is reachable
- Swagger/OpenAPI loads
- workers start successfully
- local storage dependencies are available
- authentication bootstrap works if you are keeping the default auth setup

## 11. Bootstrap The Frontend If Needed

If the project includes a SPA:

1. create a separate frontend repository
2. bootstrap it from the frontend template referenced in the wiki
3. set the frontend `apiUrl`
4. set backend CORS origins
5. set backend `ClientApplication:Url`

Important URL rule:

- frontend `apiUrl` should end with `/`
- backend CORS origins should not end with `/`

## 12. Build The First Feature Using Obelisk Conventions

Follow the default feature path:

1. decide whether the feature needs a command, a query, or both
2. put commands in `Domain`
3. put handlers and query DTOs in `Application`
4. keep controllers thin in `WebApi`
5. keep persistence concerns in `Infrastructure`
6. add or update tests in the matching test project

Preferred placement:

- commands: `src/<Project>.Domain/Features/.../Commands`
- command handlers: `src/<Project>.Application/Features/.../CommandHandlers`
- query DTOs and query handlers: `Application`
- controllers: `src/<Project>.WebApi/Controllers`
- migrations and persistence: `Infrastructure`

## 13. Refresh Generated Facts When The Surface Changes

Refresh the Context Pack whenever you change:

- solution structure
- package references
- appsettings or host/local settings
- authentication setup
- controller auth surface

Command:

```powershell
dotnet run --project tools/Obelisk.ContextPack -- generate-template --repo .
```

## 14. Keep Quality Gates In Place From Day 0

Before merging work:

- keep the solution build green
- run relevant tests
- update generated Context Pack facts when needed
- get review for auth-sensitive and security-sensitive changes

## 15. Suggested First AI Prompt

Use this in the new project repository after filling in the project profile and glossary:

```text
Read docs/context/overview.md, docs/context/architecture.md, docs/context/_generated/repo-map.md, docs/context/project-profile.md, and docs/context/domain-glossary.md. Summarize the current project shape, identify review-heavy areas, and scaffold the first feature following Obelisk conventions: commands in Domain, handlers in Application, thin controller in WebApi, and matching tests.
```

If Codex skills are available, you can also start with:

```text
Use $bootstrap-obelisk-greenfield to bootstrap a new AI-assisted Obelisk project.
```

## Sources

- Wiki bootstrap:
  - `D:\_Obelisk\NEMO-Pro.wiki\Obelisk\New-project-setup\Setup-and-run-Backend-(WebAPI-+-Workers).md`
  - `D:\_Obelisk\NEMO-Pro.wiki\Obelisk\New-project-setup\Setup-and-run-Frontend-(WebUI-SPA).md`
  - `D:\_Obelisk\NEMO-Pro.wiki\Obelisk\New-project-setup\Setup-connection-between-Frontend-and-Backend.md`
  - `D:\_Obelisk\NEMO-Pro.wiki\Obelisk\Testing-Strategy.md`
- Context Pack:
  - `docs/context/overview.md`
  - `docs/context/architecture.md`
  - `docs/context/ai-working-agreement.md`
  - `docs/context/configuration-and-environments.md`
  - `docs/context/feature-delivery-guide.md`
  - `docs/context/testing-and-quality-gates.md`
