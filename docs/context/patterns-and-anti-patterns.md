# Patterns And Anti-Patterns

## Preferred patterns

- Thin controllers that translate HTTP input into mediator requests
- Commands in `Domain`, handlers in `Application`
- Feature-oriented foldering under `Features/...`
- Explicit auth and role restrictions at controller or action level
- Environment-driven switching for email, storage, and related infrastructure services
- Central package management through `Directory.Packages.props`
- Generated fact sheets for repository truth plus handwritten guidance for intent

## Good signs in a change

- the feature sits in the existing layer structure
- the controller does not contain business logic
- new config follows the existing options-section shape
- auth requirements are visible in attributes or startup wiring
- generated docs are refreshed when surface facts change

## Anti-patterns to avoid

- putting business logic directly in controllers
- introducing new folder structures that bypass the established feature layout
- coupling runtime code to local-only assumptions without environment guards
- changing auth behavior without documenting the intent
- adding package versions directly in project files when central package management should own them
- treating repo-derived facts as handwritten prose instead of generating them
- inferring “preferred” behavior from legacy code without confirming it in guidance

## Review-heavy areas

- authentication and authorization
- migrations and persistence model changes
- domain-event and queue handling
- email flows and token-based links
- deployment-sensitive configuration
