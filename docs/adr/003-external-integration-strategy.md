# ADR-003: External Integration Strategy

## Status

Accepted

## Context

The Order Processing Platform integrates with external services for inventory availability, payment processing, and shipping. These external APIs may change providers, have different SLAs, and require resilience patterns (retry, timeout, circuit-breaker). The integration approach must:

- Isolate external API details from business logic
- Allow mocking for testing
- Support provider changes without modifying application or domain code
- Enable resilience configuration per external service

## Decision

We use the **Ports and Adapters** pattern for all external integrations:

1. **Application layer** defines ports (abstractions): `IInventoryService`, `IPaymentService`, `IShippingService`, `IPaymentGateway`, `IShippingCarrierService`
2. **Infrastructure layer** implements adapters using **Refit** as the HTTP client library
3. Refit interfaces (e.g., `IInventoryApi`, `IPaymentGatewayApi`) are infrastructure-only concerns
4. Adapters map between Refit responses and application-level types

```
Application Layer          Infrastructure Layer           External
─────────────────         ──────────────────────         ────────
IInventoryService    →    InventoryServiceClient    →    Inventory API
IPaymentGateway      →    PaymentGatewayClient      →    Payment Provider
IShippingCarrierService → ShippingCarrierClient     →    Shipping Carrier
```

Refit clients are registered via `IHttpClientFactory` with per-client configuration for base URL, timeout, and authentication.

## Consequences

**Benefits:**

- Application and domain layers have zero knowledge of Refit, HTTP, or external API shapes
- Providers can be swapped by implementing a new adapter without changing business logic
- Unit tests mock the port interface, not HTTP calls
- Resilience policies (Polly) can be configured per `HttpClient` without touching application code

**Risks:**

- Adapter layer adds mapping code between Refit responses and application DTOs
- Refit's code generation requires interfaces to match external API contracts exactly

## Alternatives Considered

**Direct HttpClient usage:** Rejected. More boilerplate for request/response serialization, header management, and error handling. Refit provides a declarative, type-safe alternative.

**Calling external APIs directly from Application layer:** Rejected. Violates dependency inversion — application logic would depend on HTTP client infrastructure, making it untestable and tightly coupled to specific providers.
