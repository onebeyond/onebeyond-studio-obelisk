# Architecture

## Runtime topology

This template is orchestrated by Aspire in `src/OneBeyond.Studio.Obelisk.AppHost`.

The running system is split into:

- `WebApi`: the HTTP surface for external clients
- `Workers`: Azure Functions for background processing
- Azurite storage emulator: local queue/blob/table storage used during development

The AppHost starts the API and workers, provisions emulator storage, and wires host storage for the worker process.

## Layer responsibilities

- `Domain`: commands, entities, domain events, and business-facing types
- `Application`: command/query handlers, DTOs, services, and projection registration
- `Authentication`: identity, JWT, cookies, Azure AD, and related auth services
- `Infrastructure`: EF Core data access, repositories, migrations, seeding, and integrations
- `WebApi`: HTTP controllers, OpenAPI, middleware, security headers, and endpoint wiring
- `Workers`: background handlers such as domain event processing and JWT cleanup
- `ServiceDefaults`: Aspire service defaults and common host behavior

## Request and command flow

The dominant backend path is:

1. Controller receives HTTP input.
2. DTO or mixed-source command is bound in `WebApi`.
3. Controller sends a command or query through mediator.
4. Handler lives in `Application`.
5. Domain entities and commands live in `Domain`.
6. Persistence and projections are handled through infrastructure-backed repositories.

## Background and integration flow

The template supports queued domain-event processing.

- `WebApi` configures Azure queue support for raised domain events.
- `DomainEventRelayJob` relays events from the receiver host.
- `Workers` host background processors, including the queue-based domain-event processor.

## Cross-cutting architecture

- DI is built with Autofac plus service registrations in the startup layers.
- OpenAPI is versioned and currently exposes `v1`.
- File storage switches between filesystem in development and Azure Blob Storage outside development.
- Email switches between folder output in development and SendGrid outside development.
- Key Vault configuration can be enabled for production environments.

## Frontend boundary

The client application is treated as a separate concern.
This backend template knows the client only through `ClientApplication` URL settings used to generate password reset and set-password links.
