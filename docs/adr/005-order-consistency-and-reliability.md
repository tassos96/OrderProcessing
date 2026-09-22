# ADR-005: Order Consistency and Reliability

## Status

Accepted

## Context

Order processing involves multiple steps that span internal state changes and external service calls (inventory reservation, payment authorization, shipment creation). These steps cannot participate in a single database transaction. The system must handle:

- Partial failures (e.g., payment fails after inventory is reserved)
- External API unavailability
- Duplicate requests (network retries, user double-clicks)
- Reliable event publishing for downstream consumers

## Decision

We adopt the following reliability patterns:

### 1. Application-Level Orchestration

The `CreateOrderCommandHandler` orchestrates the order creation flow sequentially. The application layer owns the workflow — the domain layer does not call external services.

### 2. Outbox Pattern for Reliable Event Publishing

Domain events are persisted to an `OutboxMessage` table within the same database transaction as the aggregate state change. A background processor publishes outbox messages to a message broker, ensuring at-least-once delivery.

```
Domain Event → Outbox Table (same transaction) → Background Publisher → Message Broker
```

### 3. Idempotency

Order creation accepts an `Idempotency-Key` (via the request `RequestId`). Duplicate requests with the same key return the original result without re-executing the operation.

### 4. Compensating Actions

If a downstream step fails (e.g., payment fails after inventory reservation), the handler executes compensating actions (e.g., release inventory). This follows the Saga pattern with orchestration.

### 5. Resilience for External Calls

External API calls are wrapped with retry, timeout, and circuit-breaker policies (via `Microsoft.Extensions.Http.Resilience` or Polly). Configuration is per-provider based on their SLA.

## Consequences

**Benefits:**

- Database consistency within each module (single transaction per aggregate)
- Reliable event delivery via outbox (survives process crashes)
- Idempotency prevents duplicate order creation
- Compensating actions handle partial failures gracefully
- Resilience policies protect against transient external failures

**Risks:**

- Eventual consistency — downstream consumers may see stale data briefly
- Outbox processing adds latency to event delivery
- Compensating actions add complexity and must be tested thoroughly

## Alternatives Considered

**Distributed Transactions (2PC):** Rejected. External payment and shipping APIs do not support distributed transactions. Even within the monolith, 2PC adds significant complexity and reduces throughput.

**Choreography-Only (Event-Driven):** Rejected as primary approach. While suitable for loosely coupled systems, choreography makes the order workflow harder to reason about, debug, and monitor. Orchestration provides a clear, sequential flow with explicit error handling. Choreography can be added later for cross-module integration events.
