# Source Notes

This skill was derived from two source families that serve different roles:

## Wiki Bootstrap

Use the wiki-derived flow for:

- creating the initial backend project from the template generator
- understanding the historical backend/frontend split
- wiring frontend and backend URLs
- capturing broader testing expectations

## Repo-Local Context Pack

Use the Context Pack for:

- the current architecture and layering rules
- AI working boundaries
- repository facts from generated docs
- the preferred local runtime shape

## Important Divergence

The main source divergence is local startup shape:

- the older wiki flow describes running `WebApi` and `Workers` more directly
- the current Context Pack describes the template as Aspire-orchestrated through `AppHost`

For new greenfield projects, prefer `AppHost` unless the generated project or user instructions clearly say otherwise.

## Environment-Specific Paths

Some local environments may expose paths such as:

- a local clone of the Obelisk template repo
- a local clone of the Obelisk wiki
- a local copy of the template generator

Treat those as environment-specific aids, not universal assumptions. When they are unavailable, ask for the path or fall back to the corresponding remote repository or user-provided source.
