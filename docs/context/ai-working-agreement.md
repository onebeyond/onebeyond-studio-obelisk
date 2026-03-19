# AI Working Agreement

## What AI can lead safely

- DTO creation and straightforward endpoint plumbing
- routine command/query scaffolding that follows existing feature structure
- documentation updates
- generated fact refreshes
- boilerplate tests

## What AI can help with but needs careful review

- authentication and authorization changes
- migrations and persistence changes
- queue or domain-event handling
- file storage and email flows
- configuration changes that affect deployment or secrets

## What should remain engineer-led

- aggregate and domain-model design
- business invariants and policy-heavy logic
- security-sensitive changes
- changes that redefine system boundaries
- project-specific divergence decisions in brownfield systems

## How AI should work in this repo

- start from `docs/context/overview.md`
- prefer `_generated` facts over inference
- follow the existing layer and feature structure
- call out ambiguity instead of silently choosing a pattern
- refresh generated facts when repo/config/auth surface changes

## Human responsibilities

- review review-heavy and engineer-led categories
- decide when legacy code is not the preferred pattern
- keep handwritten docs aligned with the intended golden path
