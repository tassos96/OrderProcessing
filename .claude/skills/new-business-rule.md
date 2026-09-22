# New Business Rule

Add a business rule in the architecturally correct layer with proper validation, exceptions, and testing.

## When to use

- Adding a domain invariant (e.g., "an order cannot be cancelled after shipping")
- Adding a validation constraint (e.g., "order total must not exceed $10,000")
- Adding an orchestration rule (e.g., "inventory must be reserved before payment")

## Before you start

1. Read the target entity to understand its current state machine and domain events:
   - e.g., `src/Modules/Orders/Orders.Domain/Entities/Order.cs` for order rules
2. Read `src/BuildingBlocks/BuildingBlocks.Domain/Exceptions/DomainException.cs` for the exception hierarchy.
3. Read the module's existing domain exception (e.g., `Orders.Domain/Exceptions/OrderDomainException.cs`).
4. Read existing domain events in the module's `Events/` folder.
5. Read the relevant handler to understand how the rule fits into the orchestration flow.

## Rule placement decision

| Rule type | Where it goes | Example |
|---|---|---|
| **Aggregate invariant** — validates state within a single entity | Domain entity method | `Order.Cancel()` checks status before allowing cancellation |
| **Input validation** — validates command/request data | Application validator (`AbstractValidator<T>`) | `CreateOrderCommandValidator` checks required fields |
| **Cross-aggregate / service-dependent** — requires data from outside the entity | Application handler | Handler checks inventory availability before creating order |
| **Cross-module** — depends on another module's state | Application handler via port interface | Handler calls `IInventoryService.ReserveStockAsync()` |

## Steps

1. **Classify the rule** using the table above. Explain to the developer WHY the rule belongs in that layer.

2. **For domain aggregate invariants:**
   - Add a method on the entity (or modify an existing one) with a guard clause:
     ```
     if (Status is OrderStatus.Shipped or OrderStatus.Completed)
         throw new OrderDomainException($"Cannot cancel order in status '{Status}'.");
     ```
   - Follow the pattern from `Order.Cancel()` in `src/Modules/Orders/Orders.Domain/Entities/Order.cs`
   - If the rule involves a state transition, update the entity's status and raise a domain event via `AddDomainEvent()`
   - Create or reuse the module's domain exception class (`sealed class {Module}DomainException : DomainException`)

3. **For input validation rules:**
   - Add rules to the existing `AbstractValidator<TCommand>` in the Application layer
   - Use FluentValidation fluent API: `.NotEmpty()`, `.GreaterThan()`, `.Must()`, `.WithMessage()`
   - Follow the pattern from `CreateOrderCommandValidator`

4. **For application orchestration rules:**
   - Add logic in the handler, before or after domain operations
   - Use port interfaces for external dependencies (e.g., `IInventoryService`)
   - If the rule fails, throw `DomainException` or return an error DTO — do NOT use try/catch for control flow in the controller

5. **Document the rule** by writing a clear comment in the entity method or handler explaining the business rationale (only if the WHY is non-obvious from the method name and guard clause).

## Architecture rules

Business rules must NEVER be placed in:
- **Controllers** — controllers delegate, they don't decide
- **Repositories** — repositories persist, they don't validate
- **Infrastructure adapters** — adapters translate, they don't enforce rules
- **External service clients** — clients communicate, they don't contain business logic
- **EF Core configurations** — configurations map, they don't constrain behavior

## Checklist

- [ ] Rule placed in the correct architectural layer
- [ ] Explanation provided for WHY it's in that layer
- [ ] Domain exception used for domain rule violations (not `ArgumentException` or `InvalidOperationException`)
- [ ] Domain event raised if a state transition occurs
- [ ] Unit test added covering both valid and invalid cases
- [ ] `dotnet build` succeeds
- [ ] `dotnet test` passes

## Things NOT to do

- Do not add business rules to controllers or repositories
- Do not introduce new exception types unless the module doesn't have one yet
- Do not implement compensating actions unless ADR-005 patterns are already in place
- Do not add cross-module references — use Contracts for cross-module communication

## Related skills

- `/add-tests` — every business rule needs tests
- `/domain-event` — if the rule triggers a state transition
- `/refactor` — if the rule reveals misplaced logic
- `/architecture-check` — verify no dependency violations
