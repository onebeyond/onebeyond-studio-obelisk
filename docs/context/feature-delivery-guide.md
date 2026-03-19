# Feature Delivery Guide

Use this guide for new backend work that should follow the existing Obelisk template shape.

## Default path for a new backend feature

1. Decide whether the change is a command, a query, or both.
2. Put command types in `Domain` and query DTOs/handlers in `Application`.
3. Add or extend domain entities in `Domain`.
4. Implement handlers in `Application`.
5. Use infrastructure-backed repositories and projections instead of bypassing the established data-access shape.
6. Expose the feature through a `WebApi` controller.
7. Add or update tests in the matching test project.
8. Refresh generated Context Pack facts if the repo/auth/config surface changed.

## Placement rules

- Commands belong in `src/OneBeyond.Studio.Obelisk.Domain/Features/.../Commands`
- Command handlers belong in `src/OneBeyond.Studio.Obelisk.Application/Features/.../CommandHandlers`
- Query DTOs and query handlers belong in `Application`
- Controllers belong in `src/OneBeyond.Studio.Obelisk.WebApi/Controllers`
- EF Core migrations and persistence concerns belong in `Infrastructure`

## Controller conventions

- Keep controllers thin.
- Bind HTTP input, then hand off to mediator.
- Prefer returning framework results such as `CreatedAtAction`, `NoContent`, or typed DTO responses.
- Use `[FromMixedSource]` only when the command is intentionally assembled from route plus body.

## Domain and application conventions

- Validate input in command constructors and handlers with the existing guard style.
- Let handlers orchestrate repositories and services.
- Keep business meaning in the domain model instead of burying it in controllers.
- Prefer extending existing feature folders over inventing parallel structures.

## Persistence and integrations

- Use the registered data-access abstractions and projections.
- Treat migrations, queue wiring, file storage, and email flows as review-heavy changes.
- Treat auth-sensitive changes as review-heavy even if the code change is small.

## Before opening a PR

- run the relevant tests
- refresh `docs/context/_generated` if facts changed
- check whether the new work should be documented in the worked example or pattern guides
