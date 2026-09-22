# ADR-001: Modular Monolith Architecture

## Status

Accepted

## Context

We are building an Order Processing Platform that integrates multiple business capabilities: orders, inventory, pricing, payments, and shipping. We need an architecture that provides clear module boundaries, enables independent development within modules, and supports future extraction into distributed services if the need arises.

Key constraints:

- Single development team with shared codebase ownership
- Shared SQL Server database is operationally feasible
- Deployment simplicity is valued — one artifact to build, test, and deploy
- Module boundaries must be explicit and enforceable
- The architecture must support future decomposition without a rewrite

## Decision

We adopt a **Modular Monolith** architecture with five business modules (Orders, Inventory, Pricing, Payments, Shipping) deployed as a single ASP.NET Core application.

Each module has its own internal layered structure (Domain, Application, Infrastructure, Contracts) and owns its own database schema. Modules communicate via well-defined contracts (integration events), not by reaching into each other's internals. Module boundaries are enforced by project references and validated by automated architecture tests (NetArchTest).

## Consequences

**Benefits:**

- Single deployment unit — simpler CI/CD, no distributed system coordination
- Shared process — in-memory communication, no network latency between modules
- Strong module boundaries — enforced by project structure and architecture tests
- Future extraction — modules can be promoted to independent services by replacing in-process communication with messaging
- Easier debugging and local development

**Risks:**

- Shared process means one module's failure can affect others (mitigated by exception handling and circuit breakers)
- Shared database means schema migrations require coordination (mitigated by per-module schemas)
- Team discipline required to maintain module boundaries (mitigated by architecture tests)

## Alternatives Considered

**Microservices:** Rejected. The team size and operational maturity do not justify the overhead of distributed deployment, service discovery, network resilience, and independent data stores at this stage. Premature distribution adds complexity without proportional benefit.

**Traditional Layered Monolith:** Rejected. While simpler initially, a single shared domain layer leads to tangled dependencies and makes future decomposition extremely difficult. Explicit module boundaries are a requirement.
