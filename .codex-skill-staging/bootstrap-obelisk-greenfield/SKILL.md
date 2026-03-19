---
name: bootstrap-obelisk-greenfield
description: Bootstrap new greenfield projects from the One Beyond Obelisk template, combining wiki bootstrap steps with repo-local Context Pack guidance. Use when Codex needs to create a new AI-assisted Obelisk project, explain the day-0 setup flow, scaffold project bootstrap docs, or guide backend/frontend setup, configuration, and first-feature delivery for a fresh Obelisk codebase.
---

# Bootstrap Obelisk Greenfield

Help the user start a new Obelisk-based greenfield project with the current recommended workflow.

Keep the workflow practical:

- use the wiki bootstrap steps for initial project creation
- use the generated project's `AGENTS.md` and `docs/context/` as the source of truth once the repo exists
- prefer the current `AppHost`-based local runtime shape when the sources diverge

## Gather Inputs

Collect or infer the minimum inputs:

- project name
- backend repo location or URL
- whether a frontend SPA is needed
- frontend repo location or URL if applicable
- local template and wiki paths if the user wants you to execute the bootstrap locally

If the user does not provide machine-specific paths, do not invent them as global truths. Use local defaults only when they are present on disk and clearly described as environment-specific.

## Build Context In Order

When working inside an existing Obelisk repo:

1. Read `AGENTS.md` first if present.
2. Read `docs/context/overview.md`.
3. Prefer `docs/context/_generated/` for factual repository details.
4. Use handwritten docs in `docs/context/` for conventions and AI boundaries.

Load [references/day0-checklist.md](references/day0-checklist.md) for the default day-0 workflow.
Load [references/source-notes.md](references/source-notes.md) when you need the source rationale or need to explain why `AppHost` is preferred over the older wiki startup flow.

## Execute The Bootstrap Workflow

Follow this sequence:

1. Create the backend repo on branch `main`.
2. If needed, create a separate frontend repo.
3. Bootstrap the backend from the Obelisk template and template generator.
4. Push the generated backend into the new repo.
5. Install or verify local prerequisites.
6. Create project-specific AI context files such as `docs/context/project-profile.md` and `docs/context/domain-glossary.md`.
7. Configure baseline application settings.
8. Run the backend locally through `AppHost`.
9. Verify the baseline before custom feature work.
10. Bootstrap and wire the frontend only if requested.
11. Build the first feature using standard Obelisk layer placement.
12. Refresh generated Context Pack facts when the repo/config/auth surface changes.

## AI Boundaries

AI can lead:

- DTO creation
- straightforward endpoint plumbing
- routine command/query scaffolding
- documentation updates
- boilerplate tests
- generated-fact refreshes

Flag for human review:

- aggregate and domain-model design
- business invariants
- authentication and authorization changes
- migrations and persistence changes
- queue or domain-event handling
- email, storage, and deployment-sensitive configuration

## Expected Outputs

Default to producing one or more of these:

- a day-0 checklist tailored to the user's environment
- a `DAY0.md` bootstrap document
- `project-profile.md` and `domain-glossary.md` starter files
- exact commands for local bootstrap when paths are available
- a first AI prompt for feature scaffolding in the new repo

Keep the response explicit about which steps come from the wiki bootstrap and which come from the repo-local Context Pack.
