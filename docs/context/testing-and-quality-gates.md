# Testing And Quality Gates

## Current automated shape

- the repository has `WebApi.Tests` and `Domain.Tests` projects
- current evidence shows only a small number of Web API middleware tests are discoverable today
- the existing CI workflow restores, builds, and runs tests across the solution

## Quality gates that matter here

- solution build must stay green
- relevant tests must pass
- generated Context Pack facts must stay in sync
- security- and auth-sensitive changes need review even when tests pass

## Testing expectations by change type

- controller and middleware changes: add or update Web API tests
- domain behavior changes: prefer domain-level tests where the shape supports them
- handler changes: cover through focused tests where possible
- generator changes: run the generator twice and confirm deterministic output

## Current gaps

- domain-level test coverage is currently thin
- the template should not pretend those tests already exist
- documentation and code review need to compensate until the test shape is stronger

## Context Pack verification

Use the dedicated Context Pack workflow to ensure:

- `_generated` outputs are refreshed
- bootstrap files still point to the correct docs
- the tool continues to run outside the main solution
