# Obelisk Context Pack Overview

This repository ships a repo-local Context Pack so engineers and AI tools can work from the same baseline without depending on tribal knowledge or external wiki pages.

## What this pack is for

- Explain the Obelisk backend golden path as it exists in this template today.
- Keep stable repository facts close to the code.
- Make common delivery work easier to do safely with AI assistance.
- Give one worked backend example that shows the intended end-to-end flow.

## How to use it

Read in this order:

1. `overview.md`
2. `architecture.md`
3. `_generated/repo-map.md`
4. `_generated/dependency-inventory.md`
5. `_generated/config-surface.md`
6. `_generated/auth-surface.md`
7. `feature-delivery-guide.md`
8. `patterns-and-anti-patterns.md`
9. `examples/create-user-flow.md`
10. `maintenance.md`

## Generated vs handwritten

Generated files live in `docs/context/_generated/`.
Use them for facts that can be derived from the repository:

- project layout
- package inventory
- configuration surface
- authentication and authorization surface

Handwritten files live in `docs/context/`.
Use them for guidance that should stay intentional:

- architecture explanation
- where to add new code
- preferred implementation patterns
- AI usage boundaries
- worked examples

## Refresh workflow

Refresh generated facts whenever you change:

- projects or solution structure
- package references or package versions
- appsettings, host, or local settings files
- authentication setup or controller auth surface

Command:

```powershell
dotnet run --project tools/Obelisk.ContextPack -- generate-template --repo .
```

## Maintenance rules

- Treat `_generated` as committed build artifacts for repository facts.
- Treat handwritten docs as the delivery contract for how new work should follow Obelisk.
- If code and docs disagree, fix the mismatch instead of leaving both versions in place.
- Refresh the Context Pack before template releases.
