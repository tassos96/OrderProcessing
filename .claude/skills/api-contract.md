# API Contract Change

Manage API contract changes with backward compatibility analysis.

## When to use

- Modifying request or response DTOs
- Changing HTTP status codes or error codes
- Adding, removing, or renaming endpoints
- Changing the envelope structure
- Modifying OpenAPI documentation

## Before you start

1. Read the affected controller(s) in `src/OrderProcessing.Api/Controllers/`.
2. Read ADR-004: `docs/adr/004-api-envelope-and-error-handling.md` for the envelope contract.
3. Read affected DTOs in `src/Modules/{Module}/{Module}.Application/DTOs/`.
4. Read `src/BuildingBlocks/BuildingBlocks.Application/Envelope/ResponseEnvelope.cs` and `RequestEnvelope.cs`.
5. Read `src/OrderProcessing.Api/Middleware/GlobalExceptionHandler.cs` for error code mapping.
6. Read `tests/OrderProcessing.ContractTests/EnvelopeContractTests.cs` for existing contract tests.

## Steps

### 1. Identify the change

What is being modified?
- Request DTO fields (added, removed, renamed, type changed)
- Response DTO fields
- HTTP method or route
- Status codes
- Error codes in `GlobalExceptionHandler`
- Envelope structure (`RequestEnvelope<T>`, `ResponseEnvelope<T>`)

### 2. Classify the change

| Classification | Criteria | Examples |
|---|---|---|
| **Non-breaking** | Additive, backward compatible | New optional field, new endpoint, new error code |
| **Potentially breaking** | Depends on consumer behavior | New required field with default, stricter validation |
| **Breaking** | Removes or changes existing contract | Removed field, renamed field, changed type, removed endpoint, changed route |

### 3. Apply the change

- **For DTO changes:** Modify the `sealed record` in `{Module}.Application/DTOs/`. New optional fields should be nullable or have defaults.
- **For new error codes:** Add to `GlobalExceptionHandler` switch expression. Follow SCREAMING_SNAKE_CASE convention (e.g., `PAYMENT_DECLINED`, `INVENTORY_INSUFFICIENT`).
- **For endpoint changes:** Update controller action with `[ProducesResponseType]` attributes reflecting actual responses.
- **For envelope changes:** Modify `RequestEnvelope<T>` or `ResponseEnvelope<T>` in `BuildingBlocks.Application.Envelope`. These affect ALL endpoints — proceed with extreme caution.

### 4. Verify envelope compliance

All endpoints must follow:
- POST/PUT: accept `RequestEnvelope<T>` body, return `ResponseEnvelope<T>`
- GET: return `ResponseEnvelope<T>`
- Errors: return `ResponseEnvelope<object>` via `GlobalExceptionHandler`
- Error codes: SCREAMING_SNAKE_CASE (e.g., `ORDER_NOT_FOUND`, `VALIDATION_ERROR`)

### 5. Update contract tests

- If DTO shape changed: consider adding serialization round-trip tests to `EnvelopeContractTests`
- If new integration event created: add assembly to `IntegrationEventContractTests`
- Run `dotnet test --filter "FullyQualifiedName~ContractTests"` to verify

### 6. Document the change

For breaking changes, explain:
- What changed and why
- Impact on existing consumers
- Migration path for consumers

## Architecture rules

- DTOs live in the **Application** layer — they are the API contract
- Envelope types live in **BuildingBlocks.Application** — they affect all modules
- Error code mapping lives in `GlobalExceptionHandler` in the API project
- Controllers must use `ResponseEnvelope<T>` for ALL responses (success and failure)
- Domain entities must NEVER be exposed through the API

## Checklist

- [ ] Change classified as breaking/non-breaking/potentially breaking
- [ ] Envelope pattern preserved for all affected endpoints
- [ ] `[ProducesResponseType]` attributes updated
- [ ] Error codes follow SCREAMING_SNAKE_CASE convention
- [ ] Contract tests pass
- [ ] Breaking changes documented with migration path
- [ ] `dotnet build` succeeds

## Things NOT to do

- Do not silently remove fields from response DTOs — this breaks consumers
- Do not change HTTP methods or routes without documenting the migration
- Do not expose domain entities through the API boundary
- Do not modify the envelope structure without assessing impact on ALL endpoints

## Related skills

- `/new-endpoint` — new endpoints establish API contracts
- `/add-tests` — contract tests verify API shape
- `/code-review` — review should assess contract impact
- `/pre-pr` — PR should document contract changes
