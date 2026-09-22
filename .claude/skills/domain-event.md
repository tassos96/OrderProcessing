# Domain Event

Add a domain event and optionally its corresponding integration event for cross-module communication.

## When to use

- A significant state change occurs in an aggregate (e.g., order confirmed, payment completed)
- Other parts of the system need to react to a domain change
- Cross-module communication is required via integration events

## Before you start

1. Read existing domain events as exemplars:
   - `src/Modules/Orders/Orders.Domain/Events/OrderCreatedDomainEvent.cs`
   - `src/Modules/Orders/Orders.Domain/Events/OrderConfirmedDomainEvent.cs`
2. Read the integration event pattern:
   - `src/Modules/Orders/Orders.Contracts/IntegrationEvents/OrderCreatedIntegrationEvent.cs`
3. Read the base interfaces:
   - `src/BuildingBlocks/BuildingBlocks.Domain/Abstractions/IDomainEvent.cs`
   - `src/BuildingBlocks/BuildingBlocks.Contracts/IIntegrationEvent.cs`
4. Read the entity where the event will be raised to understand the state transition.

## Steps

1. **Create the domain event** in `src/Modules/{Module}/{Module}.Domain/Events/`:
   ```
   public sealed record {Action}{Entity}DomainEvent(
       Guid {Entity}Id,
       // ... relevant properties
       DateTime OccurredOnUtc) : IDomainEvent;
   ```
   - Naming convention: `{Action}{Entity}DomainEvent` (e.g., `OrderShippedDomainEvent`, `PaymentRefundedDomainEvent`)
   - Always include the aggregate ID and `OccurredOnUtc` as the last parameter
   - Include only the data needed by consumers — keep it minimal

2. **Raise the event** from the entity method using `AddDomainEvent()`:
   ```
   AddDomainEvent(new {Action}{Entity}DomainEvent(Id, DateTime.UtcNow));
   ```
   - The event should be raised AFTER the state change succeeds (after guard clauses pass)
   - Follow the pattern in `Order.Confirm()` or `Order.Cancel()`

3. **If cross-module communication is needed**, create the integration event in `src/Modules/{Module}/{Module}.Contracts/IntegrationEvents/`:
   ```
   public sealed record {Action}{Entity}IntegrationEvent(
       Guid EventId,
       Guid {Entity}Id,
       // ... relevant properties
       DateTime OccurredOnUtc) : IIntegrationEvent;
   ```
   - Integration events MUST include `Guid EventId` as the first parameter
   - Integration events live in Contracts (the only cross-module visible layer)
   - They are separate from domain events — an integration event is published AFTER the domain event is handled

4. **Add a TODO for the event handler** in the Application layer:
   ```
   // TODO: Create INotificationHandler<{Action}{Entity}DomainEvent> to:
   //   - Map domain event to integration event
   //   - Persist to outbox (see BuildingBlocks.Infrastructure.Outbox.OutboxMessage)
   //   - Publish to message broker when outbox processor runs
   ```
   Do NOT implement a full event bus or outbox processor — the repository uses the outbox pattern conceptually but the processor is not yet implemented.

5. **Update contract tests** if an integration event was created:
   - Verify the new Contracts assembly is included in `IntegrationEventContractTests.ContractAssemblies` array in `tests/OrderProcessing.ContractTests/IntegrationEventContractTests.cs`
   - The existing reflection-based tests will automatically verify the event implements `IIntegrationEvent` and is `sealed`

6. **Add unit test** verifying the domain event is raised:
   ```
   order.DomainEvents.Should().ContainSingle()
       .Which.Should().BeOfType<{Action}{Entity}DomainEvent>();
   ```

## Architecture rules

- Domain events (`IDomainEvent`) live in the **Domain** layer
- Integration events (`IIntegrationEvent`) live in the **Contracts** layer
- Event handlers live in the **Application** layer (as `INotificationHandler<T>`)
- The outbox mechanism lives in **Infrastructure** (via `BaseDbContext` and `OutboxMessage`)
- Domain events must NOT reference Infrastructure or external services

## Checklist

- [ ] Domain event is a `sealed record` implementing `IDomainEvent`
- [ ] Domain event includes `OccurredOnUtc` as last parameter
- [ ] Integration event (if created) is a `sealed record` implementing `IIntegrationEvent` with `EventId`
- [ ] Event raised in entity method after state change
- [ ] Unit test verifies event is raised
- [ ] Contract tests cover new integration event assembly
- [ ] `dotnet build` succeeds

## Things NOT to do

- Do not implement a message broker integration
- Do not implement the outbox processor (it's a TODO in `BaseDbContext`)
- Do not raise events from handlers or controllers — only from domain entities
- Do not put business logic in event handlers

## Related skills

- `/new-business-rule` — domain events often accompany state transitions
- `/add-tests` — test that events are raised correctly
- `/api-contract` — integration events are part of the module contract
- `/architecture-check` — verify events are in the correct layer
