# Developer Workflow

Recommended workflow for developing features, fixing bugs, and maintaining the Order Processing Platform.

## Core Principle

Every change follows the same cycle:

```
Understand → Inspect → Implement → Test → Architecture Check → Review → PR
```

The developer skills automate and enforce this cycle.

## Getting Started Locally

1. `dotnet build`
2. `dotnet run --project src/OrderProcessing.Api`
3. Open Scalar at `http://localhost:5100/scalar/v1`
4. Get a dev token: `POST /api/dev/token` with `{ "username": "admin" }`
5. Use the token as `Authorization: Bearer <token>` for subsequent requests

Different test users have different permissions — use `GET /api/dev/users` to see all options. For example, `readonly-user` can only call GET endpoints, while `order-manager` has full order lifecycle access.

## Workflow by Task Type

### Adding a new feature

1. **Understand** — Read the relevant ADRs and existing module code
2. **Scaffold** — Use `/new-module` if a new module is needed
3. **Implement** — Use `/new-endpoint`, `/new-business-rule`, `/database-change`, `/domain-event` as needed
4. **Test** — Use `/add-tests` to add unit, integration, and architecture tests
5. **Verify** — Use `/architecture-check` to verify dependency rules
6. **Review** — Use `/code-review` for self-review
7. **Ship** — Use `/pre-pr` for final verification

### Adding an external integration

1. **Design** — Read ADR-003 and existing integration patterns
2. **Implement** — Use `/new-integration` to create the Ports & Adapters structure
3. **Test** — Use `/add-tests` to test handler with mocked port
4. **Secure** — Use `/security-review` to verify no secrets or data leaks
5. **Observe** — Use `/observability-review` to ensure correlation flows to external calls
6. **Verify** — Use `/architecture-check` to verify Refit isolation
7. **Ship** — Use `/pre-pr` for final verification

### Fixing a bug

1. **Diagnose** — Use `/troubleshoot` to trace the request flow and identify root cause
2. **Fix** — Apply the minimal fix at the correct architectural layer
3. **Test** — Use `/add-tests` to add a regression test
4. **Review** — Use `/code-review` to verify the fix
5. **Ship** — Use `/pre-pr` for final verification

### Refactoring

1. **Baseline** — Run `dotnet test` to ensure all tests pass
2. **Plan** — Use `/refactor` to identify the smallest safe change
3. **Implement** — Make incremental changes, testing after each step
4. **Verify** — Use `/architecture-check` after structural changes
5. **Ship** — Use `/pre-pr` for final verification

### Code review

1. **Review** — Use `/code-review` for structured multi-dimensional review
2. **Security** — Use `/security-review` if the change touches auth, logging, or external services
3. **Architecture** — Use `/architecture-check` if the change modifies project references or module structure

### Onboarding

1. **Learn** — Use `/onboarding` for a guided repository walkthrough
2. **Explore** — Read the ADRs in `docs/adr/` for architectural context
3. **Practice** — Try `/new-endpoint` on an existing module to learn the conventions

## Quick Commands

```bash
# Build and run
dotnet build
dotnet run --project src/OrderProcessing.Api

# Test
dotnet test                                                    # All tests
dotnet test --filter "FullyQualifiedName~UnitTests"           # Unit tests
dotnet test --filter "FullyQualifiedName~IntegrationTests"    # Integration tests
dotnet test --filter "FullyQualifiedName~ArchitectureTests"   # Architecture tests
dotnet test --filter "FullyQualifiedName~ContractTests"       # Contract tests

# API docs (when running in development)
# Scalar:  http://localhost:5100/scalar/v1
# Swagger: http://localhost:5100/swagger
# OpenAPI: http://localhost:5100/openapi/v1.json
```

## Where Code Belongs

```
Business invariant on a single aggregate  →  Domain entity method
Use case orchestrating multiple steps     →  Application command handler
Input validation                          →  Application validator (AbstractValidator<T>)
Data access contract                      →  Domain repository interface
Data access implementation                →  Infrastructure repository
External API contract                     →  Application port interface
External API implementation               →  Infrastructure adapter (Refit)
API endpoint                              →  Controller (delegates to MediatR, no business logic)
Cross-module communication                →  Contracts integration event
Architectural decision                    →  ADR in docs/adr/
```

## Dependency Direction

```
Domain ← Application ← Infrastructure
               ↑
              API

Contracts ← (cross-module only boundary)
```

**Rule:** Dependencies always point inward. Domain knows nothing about Infrastructure. Application defines abstractions (ports). Infrastructure implements them (adapters).

Enforced by architecture tests in `tests/OrderProcessing.ArchitectureTests/`.
