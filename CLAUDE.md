# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build and Run

```bash
dotnet build                                    # Build entire solution
dotnet run --project src/OrderProcessing.Api    # Run API (http://localhost:5100)
```

API docs in dev: Scalar at `/scalar/v1`, Swagger at `/swagger`, OpenAPI spec at `/openapi/v1.json`.

## Testing

```bash
dotnet test                                         # All tests
dotnet test --filter "FullyQualifiedName~UnitTests"           # Unit tests only
dotnet test --filter "FullyQualifiedName~IntegrationTests"    # Integration tests only
dotnet test --filter "FullyQualifiedName~ArchitectureTests"   # Architecture tests only
dotnet test --filter "FullyQualifiedName~ContractTests"       # Contract tests only
dotnet test --filter "FullyQualifiedName~OrderTests"          # Single test class
dotnet test --filter "FullyQualifiedName~OrderTests.MethodName" # Single test method
```

Test frameworks: xUnit, FluentAssertions, NSubstitute, NetArchTest.eNhancedEdition. Integration tests use `WebApplicationFactory` with EF Core InMemory provider.

## Architecture

**Modular Monolith** — five modules (Orders, Inventory, Pricing, Payments, Shipping) sharing a single ASP.NET Core host. Each module follows **Clean Architecture** with four layers:

- **Domain** — Entities, value objects, domain events, repository interfaces. Zero external dependencies.
- **Application** — Commands/queries (MediatR CQRS), validators (FluentValidation), DTOs, service port interfaces.
- **Infrastructure** — EF Core DbContext, repository implementations, Refit HTTP clients for external APIs, DI registration.
- **Contracts** — Integration events for cross-module communication. Only Contracts projects may be referenced by other modules.

The API host (`OrderProcessing.Api`) references only Infrastructure projects. Each module registers itself via an `Add{Module}Module()` extension method called in `Program.cs`.

**BuildingBlocks** provides shared base types: `AggregateRoot<TId>`, `Entity<TId>`, `ValueObject`, `IDomainEvent`, `ResponseEnvelope<T>`/`RequestEnvelope<T>`, `ValidationBehavior`, `LoggingBehavior`, `BaseDbContext`, outbox abstractions, `IUnitOfWork`.

## Dependency Rules (enforced by NetArchTest in ArchitectureTests)

- Domain must not reference EF Core, ASP.NET Core, or Refit
- Application defines abstractions; Infrastructure implements them
- Modules may only depend on other modules' Contracts projects, never on their Domain/Application/Infrastructure
- Controllers contain no business logic — they delegate to MediatR

## Request Flow

Controller receives `RequestEnvelope<T>` → extracts `Payload` → sends via `ISender` (MediatR) → `ValidationBehavior` runs FluentValidation → Command/Query handler executes → Controller wraps result in `ResponseEnvelope<T>`. Unhandled exceptions are caught by `GlobalExceptionHandler` and returned as `ResponseEnvelope<object>` with appropriate error codes.

## Code Conventions

- .NET 10, C#, nullable reference types enabled, warnings treated as errors
- File-scoped namespaces (`namespace Foo;` not `namespace Foo { }`)
- Primary constructors preferred (e.g., `public sealed class Foo(IDependency dep)`)
- Central package management via `Directory.Packages.props` — use `<PackageReference Include="..." />` without version in csproj files
- Solution file is `OrderProcessing.slnx` (XML-based solution format)
- Each module's DbContext uses a separate schema (e.g., `"orders"` schema for OrdersDbContext)
- Domain entities use factory methods (e.g., `Order.Create(...)`) with private constructors
- Correlation ID propagated via `X-Correlation-Id` header and Serilog `LogContext`

## Developer Skills

AI-assisted development skills are available in `.claude/skills/`. These guided workflows enforce the architecture while helping developers perform common tasks. See `docs/SKILLS.md` for the full reference.

Key skills:
- `/new-module` — scaffold a new module with all 4 layers
- `/new-endpoint` — add an API endpoint end-to-end
- `/new-business-rule` — add a business rule in the correct layer
- `/add-tests` — add appropriate tests for changed code
- `/architecture-check` — verify dependency rules and module boundaries
- `/code-review` — structured multi-dimensional code review
- `/pre-pr` — final PR readiness checklist

Workflow documentation: `docs/DEVELOPER_WORKFLOW.md` | Architecture rationale: `docs/SKILL_ARCHITECTURE.md`

## Authorization

All endpoints require JWT Bearer authentication with claim-based authorization. Policies use a `permission` claim:

| Policy | Required Claim | Controller |
|--------|---------------|------------|
| `Orders.Read` | `permission` = `orders.read` | OrdersController |
| `Orders.Write` | `permission` = `orders.write` | OrdersController |
| `Orders.Cancel` | `permission` = `orders.cancel` | OrdersController |
| `Inventory.Read` | `permission` = `inventory.read` | InventoryController |
| `Inventory.Write` | `permission` = `inventory.write` | InventoryController |
| `Pricing.Read` | `permission` = `pricing.read` | PricingController |
| `Payments.Process` | `permission` = `payments.process` | PaymentsController |
| `Shipping.Create` | `permission` = `shipping.create` | ShippingController |

When adding a new endpoint, add `[Authorize(Policy = "{Module}.{Action}")]` and register the policy in `Program.cs` with `RequireClaim("permission", "{module}.{action}")`.

## Development Authentication

In Development, the API uses a local symmetric JWT key (configured in `appsettings.Development.json`) instead of an external identity provider. Use the dev auth endpoints to obtain tokens:

- `GET /api/dev/users` — list available test users and their permissions
- `POST /api/dev/token` — get a JWT: `{ "username": "admin" }`

Six test users with different permission profiles: `admin`, `order-manager`, `warehouse-worker`, `finance-analyst`, `shipping-clerk`, `readonly-user`. These endpoints are only available when `ASPNETCORE_ENVIRONMENT=Development`.

## Skeleton Status

This is an architectural blueprint. Most business logic uses `throw new NotImplementedException()` or `// TODO:` comments. No EF Core migrations, no real external API integrations, no message broker, no Dockerfile exist yet.
