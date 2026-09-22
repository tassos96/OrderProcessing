# ADR-002: Clean Architecture and Dependency Direction

## Status

Accepted

## Context

Each module in the modular monolith needs an internal structure that provides:

- Domain isolation from infrastructure concerns (databases, HTTP clients, frameworks)
- Testability at every layer without requiring real infrastructure
- A clear dependency direction that prevents coupling to implementation details
- The ability to swap infrastructure components (e.g., changing a payment provider) without modifying domain or application logic

## Decision

We adopt **Clean Architecture / Hexagonal Architecture** principles inside each module. The dependency rule is strictly enforced: dependencies point inward.

```
Domain ← Application ← Infrastructure
                ↑
               API
```

**Layer responsibilities:**

| Layer | Contains | Depends On |
|-------|----------|------------|
| Domain | Entities, value objects, domain events, domain exceptions, repository interfaces | Nothing (foundation) |
| Application | Use cases (commands/queries), DTOs, validators, service abstractions (ports) | Domain |
| Infrastructure | EF Core DbContext, repository implementations, Refit HTTP clients (adapters) | Application, Domain |
| API | Controllers, middleware, composition root | Application, Infrastructure (for DI wiring) |

Application defines abstractions (ports). Infrastructure implements them (adapters). This is the Dependency Inversion Principle applied at the architectural level.

## Consequences

**Benefits:**

- Domain logic is testable with zero infrastructure dependencies
- Application use cases can be tested with mock implementations of ports
- Infrastructure can be swapped (e.g., different database, different payment provider) without touching business logic
- Dependency direction is enforceable via architecture tests

**Risks:**

- Additional project files and indirection compared to a flat structure
- Developers must understand and follow the dependency rule (mitigated by NetArchTest enforcement)

## Alternatives Considered

**Traditional N-Tier (Controller → Service → Repository → Database):** Rejected. Tight coupling between layers makes testing difficult and infrastructure changes risky. Domain logic tends to leak into services that depend directly on EF Core.

**Vertical Slice Architecture:** Considered but not adopted as primary pattern. While vertical slices reduce cross-cutting abstractions, they provide weaker module-internal structure. The CQRS approach within Application provides similar slice-oriented organization while maintaining clear layer boundaries.
