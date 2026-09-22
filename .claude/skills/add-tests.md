# Add Tests

Add appropriate tests for changed or new code, using the correct test type and existing conventions.

## When to use

- After adding or modifying domain entities, value objects, or business rules
- After adding a new endpoint or handler
- After adding a new module
- After modifying integration events or API contracts
- When a code review identifies missing test coverage

## Before you start

1. Identify what code changed and which layer it belongs to (Domain, Application, Infrastructure, API, Contracts).
2. Read existing test exemplars to match the conventions:
   - **Unit tests:** `tests/OrderProcessing.UnitTests/Orders/OrderTests.cs` (domain) and `CreateOrderCommandValidatorTests.cs` (validators)
   - **Integration tests:** `tests/OrderProcessing.IntegrationTests/Orders/OrdersApiTests.cs` and `Fixtures/IntegrationTestBase.cs`
   - **Architecture tests:** `tests/OrderProcessing.ArchitectureTests/DomainDependencyTests.cs`, `ApplicationDependencyTests.cs`, `ModuleBoundaryTests.cs`
   - **Contract tests:** `tests/OrderProcessing.ContractTests/IntegrationEventContractTests.cs` and `EnvelopeContractTests.cs`
3. Read the target code to understand expected behavior.

## Test type decision tree

| Changed code | Test type | Test project | Location |
|---|---|---|---|
| Domain entity, value object, domain event | Unit | `OrderProcessing.UnitTests` | `{Module}/` folder |
| FluentValidation validator | Unit | `OrderProcessing.UnitTests` | `{Module}/` folder |
| Command/Query handler | Unit | `OrderProcessing.UnitTests` | `{Module}/` folder (mock repos with NSubstitute) |
| API endpoint (controller + full flow) | Integration | `OrderProcessing.IntegrationTests` | `{Module}/` folder |
| New module or project references | Architecture | `OrderProcessing.ArchitectureTests` | Update existing test classes |
| New integration event | Contract | `OrderProcessing.ContractTests` | Update `IntegrationEventContractTests.cs` |
| Envelope or DTO shape change | Contract | `OrderProcessing.ContractTests` | Update `EnvelopeContractTests.cs` |

## Steps

1. **Determine test types needed** using the decision tree above. Multiple types may apply.

2. **For unit tests — domain entities:**
   - Create test class in `tests/OrderProcessing.UnitTests/{Module}/{EntityName}Tests.cs`
   - Test factory methods: verify initial state, domain events raised, computed properties
   - Test state transitions: verify valid transitions succeed, invalid transitions throw `{Module}DomainException`
   - Follow naming: `MethodName_Condition_ExpectedBehavior` (e.g., `Cancel_WhenPending_ShouldTransitionToCancelled`)
   - Use strict AAA with `// Arrange`, `// Act`, `// Assert` comments
   - Use FluentAssertions: `.Should().Be()`, `.Should().Throw<>()`, `.DomainEvents.Should().ContainSingle().Which.Should().BeOfType<>()`

3. **For unit tests — validators:**
   - Create test class in `tests/OrderProcessing.UnitTests/{Module}/{ValidatorName}Tests.cs`
   - Instantiate validator as field: `private readonly {Validator} _validator = new();`
   - Test invalid inputs: verify `result.IsValid.Should().BeFalse()` and `result.Errors.Should().Contain(e => e.PropertyName == "...")`
   - Test valid inputs: verify `result.IsValid.Should().BeTrue()`

4. **For unit tests — handlers:**
   - Mock dependencies with NSubstitute: `var repo = Substitute.For<IOrderRepository>();`
   - Configure mock returns: `repo.GetByIdAsync(Arg.Any<Guid>()).Returns(order);`
   - Verify interactions: `await repo.Received(1).AddAsync(Arg.Any<Order>());`

5. **For integration tests:**
   - Create test class inheriting `IntegrationTestBase` with primary constructor: `public class {Test}(OrderProcessingWebApplicationFactory factory) : IntegrationTestBase(factory)`
   - Use `Client` to make HTTP requests
   - Wrap POST payloads with `WrapInEnvelope<T>(payload)`
   - Deserialize responses with `DeserializeResponse<T>(response)`
   - Verify HTTP status codes and envelope structure

6. **For architecture test updates:**
   - Add new module's domain assembly to `DomainDependencyTests.DomainAssemblies`
   - Add new module's application assembly to `ApplicationDependencyTests.ApplicationAssemblies`
   - Add cross-module isolation tests to `ModuleBoundaryTests`

7. **For contract test updates:**
   - Add new contract assembly to `IntegrationEventContractTests.ContractAssemblies` array
   - Verify new integration events are picked up by existing reflection-based tests

8. **Run tests:** `dotnet test` to verify all new and existing tests pass.

## Architecture rules

- Test projects may reference any production project (they are outside the dependency rules).
- Unit tests should NOT require Infrastructure (no DbContext, no HTTP clients).
- Integration tests use `WebApplicationFactory<Program>` — they test the full pipeline.
- Architecture tests use `NetArchTest.Rules.Types.InAssembly()` — they verify dependency rules via reflection.

## Checklist

- [ ] Correct test type(s) selected for the changed code
- [ ] Test naming follows `MethodName_Condition_ExpectedBehavior`
- [ ] AAA pattern with `// Arrange`, `// Act`, `// Assert` comments
- [ ] FluentAssertions used (no raw `Assert.*`)
- [ ] Tests are small and focused — not exhaustive suites
- [ ] `dotnet test` passes with zero failures

## Things NOT to do

- Do not generate dozens of tests for a single change. 2-5 focused tests is typical.
- Do not introduce Testcontainers unless the repository already uses it (it does not).
- Do not mock domain entities — test them directly through their public API.
- Do not add test infrastructure (builders, factories) unless the pattern already exists.

## Related skills

- `/code-review` — review identifies missing tests
- `/new-endpoint` — new endpoints need integration tests
- `/new-business-rule` — business rules need unit tests
- `/pre-pr` — tests must pass before PR
