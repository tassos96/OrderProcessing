# New Endpoint

Add a new API endpoint end-to-end: controller action, command/query, handler, validator, and DTOs.

## When to use

- Adding a new REST endpoint (e.g., `POST /api/orders/{id}/cancel`, `GET /api/inventory/{sku}`)
- Exposing new functionality through the API

## Before you start

1. Determine which module owns this endpoint from the route (e.g., `/api/orders/` → Orders module).
2. Read the existing controller for that module in `src/OrderProcessing.Api/Controllers/{Module}Controller.cs`.
3. Read `src/OrderProcessing.Api/Controllers/OrdersController.cs` as the canonical pattern.
4. Read `src/Modules/Orders/Orders.Application/Commands/CreateOrder/` for the command pattern.
5. Read `src/BuildingBlocks/BuildingBlocks.Application/Envelope/RequestEnvelope.cs` and `ResponseEnvelope.cs` for the envelope types.
6. Determine if this is a **command** (POST/PUT/DELETE — modifies state) or **query** (GET — reads state).

## Steps

1. **Create DTOs** (if new ones are needed) in `src/Modules/{Module}/{Module}.Application/DTOs/`:
   - Use `sealed record` with positional constructor parameters
   - Request-specific DTOs go in the command file, response DTOs in the DTOs folder

2. **Create the command or query record:**
   - **Command:** `src/Modules/{Module}/{Module}.Application/Commands/{OperationName}/{OperationName}Command.cs`
     ```
     public sealed record {OperationName}Command(...) : IRequest<{ResponseDto}>;
     ```
   - **Query:** `src/Modules/{Module}/{Module}.Application/Queries/{OperationName}/{OperationName}Query.cs`
     ```
     public sealed record {OperationName}Query(...) : IRequest<{ResponseDto}?>;
     ```

3. **Create the validator** in the same folder:
   - `public sealed class {OperationName}CommandValidator : AbstractValidator<{OperationName}Command>`
   - Add validation rules in the parameterless constructor using FluentValidation fluent API
   - Validate all required fields with `.NotEmpty()`, numeric constraints with `.GreaterThan()`, etc.

4. **Create the handler** in the same folder:
   - `public sealed class {OperationName}CommandHandler(...) : IRequestHandler<{OperationName}Command, {ResponseDto}>`
   - Use primary constructor to inject dependencies (repository, IUnitOfWork, service abstractions)
   - For skeleton implementation: `throw new NotImplementedException();` with a TODO comment describing the orchestration steps

5. **Add the controller action** in `src/OrderProcessing.Api/Controllers/{Module}Controller.cs`:
   - For POST/PUT: accept `[FromBody] RequestEnvelope<{Command}>`, extract `request.Payload`, send via `sender.Send()`
   - For GET: construct query from route/query parameters, send via `sender.Send()`
   - Return `Ok(ResponseEnvelope<{Dto}>.Success(result))` for success
   - Return `NotFound(ResponseEnvelope<{Dto}>.Failure("CODE", "message"))` for not-found cases
   - Add `[Authorize(Policy = "{Module}.{Operation}")]` attribute
   - Add `[ProducesResponseType]` attributes for success and error responses

6. **Register authorization policy** in `src/OrderProcessing.Api/Program.cs` if a new policy is needed:
   - Add `.AddPolicy("{Module}.{Operation}", policy => policy.RequireClaim("permission", "{module}.{operation}"))`
   - Follow the existing pattern: policy name is `{Module}.{Operation}` (PascalCase), claim value is `{module}.{operation}` (lowercase)
   - Update `DevAuthController.cs` test users if the new permission should be available in dev mode

7. **Add repository method** if the handler needs one that doesn't exist:
   - Add method to interface in `src/Modules/{Module}/{Module}.Domain/Repositories/I{Entity}Repository.cs`
   - Add implementation in `src/Modules/{Module}/{Module}.Infrastructure/Persistence/Repositories/{Entity}Repository.cs`

8. **Verify:** Run `dotnet build`.

## Architecture rules

- Controller contains NO business logic — only `sender.Send()` delegation and envelope wrapping
- Handler lives in Application layer — it orchestrates domain operations
- Validator lives in Application layer — it runs via `ValidationBehavior` pipeline
- DTOs live in Application layer — they are the API contract
- Repository interface lives in Domain — implementation in Infrastructure

## Checklist

- [ ] Controller action uses `RequestEnvelope<T>` for POST/PUT requests
- [ ] Controller action returns `ResponseEnvelope<T>` for all responses
- [ ] Controller delegates to `ISender.Send()` with no business logic
- [ ] `[Authorize]` attribute present with appropriate policy
- [ ] `[ProducesResponseType]` attributes document success and error responses
- [ ] Validator covers all required fields
- [ ] Handler follows primary constructor injection pattern
- [ ] `CancellationToken` passed through the entire chain
- [ ] `dotnet build` succeeds

## Things NOT to do

- Do not put validation logic in the controller
- Do not put domain logic in the handler (delegate to entity methods)
- Do not return domain entities from the controller — always use DTOs
- Do not skip the envelope pattern

## Related skills

- `/new-business-rule` — add domain logic for the handler to call
- `/add-tests` — add integration tests for the new endpoint
- `/api-contract` — review backward compatibility of the new endpoint
- `/architecture-check` — verify no dependency violations
