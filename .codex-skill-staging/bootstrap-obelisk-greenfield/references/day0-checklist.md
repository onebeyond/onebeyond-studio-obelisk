# Obelisk Greenfield Day-0 Checklist

Use this checklist when a user wants to start a fresh AI-assisted Obelisk project.

## 1. Create Repositories

- Create one backend repo with default branch `main`.
- Create a separate frontend repo only if the project needs a SPA.

## 2. Bootstrap The Backend

- Get the latest Obelisk backend template source.
- Run the template generator against the template source.
- Use a project name containing only letters, digits, and full stops.
- Treat the generated output as the initial content for the new backend repo.

Example:

```powershell
git clone https://github.com/onebeyond/onebeyond-studio-obelisk C:\dev\onebeyond-studio-obelisk

C:\tools\OneBeyondStudioTemplateGenerator\OneBeyondStudioTemplateGenerator.exe `
  "C:\dev\onebeyond-studio-obelisk" `
  "C:\dev\Manga Reader" `
  "MangaReader"
```

## 3. Push The Generated Output

Example:

```powershell
git clone <your-backend-repo-url> C:\dev\MangaReader.Backend
Copy-Item "C:\dev\Manga Reader\*" "C:\dev\MangaReader.Backend" -Recurse -Force
git -C C:\dev\MangaReader.Backend add .
git -C C:\dev\MangaReader.Backend commit -m "Initial Obelisk setup"
git -C C:\dev\MangaReader.Backend push origin main
```

## 4. Verify Local Prerequisites

- .NET SDK 10
- Docker Desktop
- Azure Functions Core Tools

## 5. Read The Context Pack

In the generated backend repo, read in this order:

1. `docs/context/overview.md`
2. `docs/context/architecture.md`
3. `docs/context/_generated/repo-map.md`
4. `docs/context/_generated/dependency-inventory.md`
5. `docs/context/_generated/config-surface.md`
6. `docs/context/_generated/auth-surface.md`
7. `docs/context/feature-delivery-guide.md`

## 6. Create Project-Specific AI Context

Create:

- `docs/context/project-profile.md` from `project-profile.template.md`
- `docs/context/domain-glossary.md` from `domain-glossary.template.md`

Fill them with:

- project name and purpose
- primary users and business-critical areas
- key integrations and environments
- safe AI-assisted work
- review-heavy work
- engineer-led work
- domain terminology

## 7. Configure Baseline Settings

Review and set:

- `ConnectionStrings:ApplicationConnectionString`
- `ClientApplication:Url`
- `DomainEvents:Queue:*`
- `EmailSender:*`
- `FileStorage:*`
- `Jwt:*`
- `AzureAd:*`
- `KeyVault:*`

Typical files:

- `appsettings.json`
- `appsettings.Development.json`
- environment-specific appsettings
- `host.json`
- `local.settings.json`

Do not commit production secrets.

## 8. Run The Backend

Prefer the current template shape and run through `AppHost`.

Example:

```powershell
dotnet run --project src\<YourProject>.AppHost\<YourProject>.AppHost.csproj
```

Expected local shape:

- `WebApi`
- `Workers`
- Azurite-backed local development storage

## 9. Verify The Baseline

Before feature work, confirm:

- the solution starts cleanly
- the API is reachable
- Swagger/OpenAPI loads
- workers start successfully
- local storage dependencies are available

## 10. Bootstrap The Frontend If Needed

- Keep frontend in a separate repo.
- Set frontend `apiUrl`.
- Set backend CORS allowed origins.
- Set backend `ClientApplication:Url`.

URL rule:

- frontend `apiUrl` should end with `/`
- backend CORS origins should not end with `/`

## 11. Build The First Feature

Follow Obelisk layer placement:

- commands in `Domain`
- handlers and query DTOs in `Application`
- thin controllers in `WebApi`
- persistence concerns in `Infrastructure`
- tests in the matching test project

## 12. Refresh Generated Facts

Run this after repo/config/auth surface changes:

```powershell
dotnet run --project tools/Obelisk.ContextPack -- generate-template --repo .
```

## 13. Suggested First AI Prompt

```text
Read docs/context/overview.md, docs/context/architecture.md, docs/context/_generated/repo-map.md, docs/context/project-profile.md, and docs/context/domain-glossary.md. Summarize the current project shape, identify review-heavy areas, and scaffold the first feature following Obelisk conventions: commands in Domain, handlers in Application, thin controller in WebApi, and matching tests.
```
