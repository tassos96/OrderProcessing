# New Developer Onboarding

Guided walkthrough of the Order Processing Platform repository for new developers.

## When to use

- First time working in this repository
- Need a refresher on the architecture
- Onboarding a new team member

## Steps

When invoked, walk the developer through the following areas by reading and explaining actual repository files.

### 1. Architecture overview

Read and explain `CLAUDE.md` — this is the primary reference. Then read ADR-001 (`docs/adr/001-modular-monolith-architecture.md`) and ADR-002 (`docs/adr/002-clean-architecture-and-dependency-direction.md`).

Key points to convey:
- **Modular Monolith** — 5 modules (Orders, Inventory, Pricing, Payments, Shipping) in a single ASP.NET Core host
- **Clean Architecture per module** — 4 layers with strict dependency direction:
  ```
  Domain ← Application ← Infrastructure
                 ↑
                API
  ```
- **BuildingBlocks** — shared base types used by all modules
- **Contracts** — the ONLY allowed cross-module dependency

### 2. Module structure

Walk through the Orders module as the canonical example. Read and explain:
- `src/Modules/Orders/Orders.Domain/Entities/Order.cs` — aggregate root with factory method, state machine, domain events
- `src/Modules/Orders/Orders.Application/Commands/CreateOrder/CreateOrderCommand.cs` — MediatR CQRS command
- `src/Modules/Orders/Orders.Application/Commands/CreateOrder/CreateOrderCommandValidator.cs` — FluentValidation
- `src/Modules/Orders/Orders.Application/Commands/CreateOrder/CreateOrderCommandHandler.cs` — use case orchestration
- `src/Modules/Orders/Orders.Infrastructure/Extensions/ServiceCollectionExtensions.cs` — DI registration pattern
- `src/Modules/Orders/Orders.Contracts/IntegrationEvents/OrderCreatedIntegrationEvent.cs` — cross-module events

### 3. Request flow

Read and trace the full flow through `src/OrderProcessing.Api/Controllers/OrdersController.cs`:
```
Client sends POST /api/orders with RequestEnvelope<CreateOrderCommand>
  → OrdersController.Create() extracts request.Payload
  → ISender.Send() dispatches to MediatR pipeline
  → ValidationBehavior runs CreateOrderCommandValidator
  → CreateOrderCommandHandler executes business logic
  → Controller wraps result in ResponseEnvelope<OrderDto>.Success()
```

Read `src/OrderProcessing.Api/Middleware/GlobalExceptionHandler.cs` to explain error handling:
- `ValidationException` → 400 VALIDATION_ERROR
- `NotFoundException` → 404 NOT_FOUND  
- `DomainException` → 422 DOMAIN_ERROR
- Unhandled → 500 INTERNAL_ERROR

### 4. How to run

```bash
dotnet build                                    # Build the solution
dotnet run --project src/OrderProcessing.Api    # Run the API (http://localhost:5100)
```

API documentation available in development:
- Scalar: `/scalar/v1`
- Swagger: `/swagger`
- OpenAPI spec: `/openapi/v1.json`

### 4a. Getting a dev token

All endpoints require JWT authentication. In Development mode, use the built-in token issuer:

1. `GET /api/dev/users` — see available test users and their permissions
2. `POST /api/dev/token` with `{ "username": "admin" }` — get a signed JWT
3. Add the token as `Authorization: Bearer <token>` header to subsequent requests

Six test users are available: `admin` (all permissions), `order-manager`, `warehouse-worker`, `finance-analyst`, `shipping-clerk`, `readonly-user`. Use different users to test permission boundaries.

### 5. How to test

```bash
dotnet test                                                    # All tests
dotnet test --filter "FullyQualifiedName~UnitTests"           # Unit tests
dotnet test --filter "FullyQualifiedName~IntegrationTests"    # Integration tests
dotnet test --filter "FullyQualifiedName~ArchitectureTests"   # Architecture tests
dotnet test --filter "FullyQualifiedName~ContractTests"       # Contract tests
```

Explain what each test project covers:
- **UnitTests** — domain entities, value objects, validators (folders per module)
- **IntegrationTests** — HTTP endpoint testing via `WebApplicationFactory`
- **ArchitectureTests** — dependency rule enforcement via NetArchTest
- **ContractTests** — API envelope and integration event shape validation

### 6. Where code belongs

Decision tree for placing new code:
- **Business invariant on a single aggregate** → Domain entity method
- **Use case orchestrating multiple steps** → Application command handler
- **Input validation** → Application validator (`AbstractValidator<T>`)
- **Data access** → Domain defines interface, Infrastructure implements it
- **External API call** → Application defines port interface, Infrastructure implements adapter with Refit
- **Cross-module communication** → Integration event in Contracts project
- **API endpoint** → Controller in `OrderProcessing.Api` (delegates to MediatR, no business logic)

### 7. Key ADRs

Point the developer to read:
- ADR-003 (`docs/adr/003-external-integration-strategy.md`) — Ports & Adapters with Refit
- ADR-004 (`docs/adr/004-api-envelope-and-error-handling.md`) — Request/Response envelope
- ADR-005 (`docs/adr/005-order-consistency-and-reliability.md`) — Outbox, idempotency, compensation

### 8. Available skills

Explain the developer skills toolkit:
- Use `/new-module` to scaffold a new module
- Use `/new-endpoint` to add an API endpoint
- Use `/new-business-rule` to add a domain rule
- Use `/add-tests` to add tests for changed code
- Use `/architecture-check` to verify dependency rules
- Use `/pre-pr` for the PR readiness checklist
- See `docs/SKILLS.md` for the complete reference

## Related skills

- `/architecture-check` — verify understanding of dependency rules
- `/new-module` — first structural task for many developers
- `/create-adr` — understand the ADR format
