# Troubleshoot

Systematic debugging by tracing the request flow from API to database.

## When to use

- An endpoint returns an unexpected error (400, 404, 422, 500)
- A feature doesn't behave as expected
- An external integration is failing
- Tests are failing for unclear reasons

## Before you start

1. Identify the endpoint, HTTP method, and module involved.
2. Read the relevant controller in `src/OrderProcessing.Api/Controllers/`.
3. Read `CLAUDE.md` "Request Flow" section for the processing pipeline.
4. If logs are available, search for the `CorrelationId` to trace the request.

## Steps

### 1. Reproduce the issue

Identify the exact request (method, URL, body) and the expected vs. actual response. Determine the HTTP status code:
- **400** → Validation failed
- **401** → Authentication failed
- **403** → Authorization failed
- **404** → Entity not found
- **422** → Domain rule violation
- **500** → Unhandled exception

### 2. Trace the request flow

Follow the pipeline in order, stopping at the first layer that shows the problem:

**Layer 1 — Middleware**
- `CorrelationIdMiddleware` — Does the request reach the pipeline?
- `GlobalExceptionHandler` — Is an exception being caught and mapped?

**Layer 2 — Controller**
- `src/OrderProcessing.Api/Controllers/{Module}Controller.cs`
- Is the route correct? Is the HTTP method correct?
- Is `[Authorize]` blocking the request? Check authorization policies in `Program.cs`.
- For 401: verify the JWT token is present and signed with the correct key. In Development, check `appsettings.Development.json` for `Authentication:Jwt:SigningKey`. Use `GET /api/dev/users` and `POST /api/dev/token` to get a valid dev token.
- For 403: verify the token contains the required `permission` claim. Decode the JWT and compare claims to the policy's `RequireClaim` in `Program.cs`.
- Is `RequestEnvelope<T>` being deserialized correctly?

**Layer 3 — Validation**
- `BuildingBlocks.Application.Behaviors.ValidationBehavior` intercepts all commands
- Read the validator: `src/Modules/{Module}/{Module}.Application/Commands/{Name}/{Name}CommandValidator.cs`
- Is the validation too strict or missing a rule?
- A `ValidationException` maps to 400 VALIDATION_ERROR

**Layer 4 — Handler**
- `src/Modules/{Module}/{Module}.Application/Commands/{Name}/{Name}CommandHandler.cs`
- Is the handler implemented or still `throw new NotImplementedException()`?
- Are dependencies resolved? Check DI registration in `src/Modules/{Module}/{Module}.Infrastructure/Extensions/ServiceCollectionExtensions.cs`
- Is `IUnitOfWork.SaveChangesAsync()` called?

**Layer 5 — Domain**
- `src/Modules/{Module}/{Module}.Domain/Entities/`
- Is the entity's state machine correct? Check guard clauses.
- A `DomainException` maps to 422 DOMAIN_ERROR
- A `NotFoundException` maps to 404 NOT_FOUND

**Layer 6 — Repository / DbContext**
- `src/Modules/{Module}/{Module}.Infrastructure/Persistence/Repositories/`
- Is the repository method implemented or `throw new NotImplementedException()`?
- Is the DbContext registered with the correct connection string?
- Check `appsettings.json` connection string: `ConnectionStrings:{Module}Db`

**Layer 7 — External services**
- `src/Modules/{Module}/{Module}.Infrastructure/ExternalServices/`
- Is the Refit client configured with the correct base URL?
- Is the adapter implemented or skeleton?
- Check `appsettings.json` section `ExternalServices:{Service}`

### 3. Check DI registration

Many issues stem from missing DI registration. Verify in `src/Modules/{Module}/{Module}.Infrastructure/Extensions/ServiceCollectionExtensions.cs`:
- DbContext registered
- Repository registered (`AddScoped<I{Entity}Repository, {Entity}Repository>`)
- UnitOfWork registered
- Service adapters registered
- Module registration called in `Program.cs`

### 4. Check configuration

Verify in `src/OrderProcessing.Api/appsettings.json`:
- Connection string exists for the module
- External service URLs are configured
- JWT configuration is valid

### 5. Report findings

```
Troubleshooting Report
══════════════════════
Symptom:    {description}
Endpoint:   {method} {url}
Module:     {module}
Layer:      {where the problem was found}

Root cause: {explanation with evidence}
Evidence:   {file}:{line} — {what was found}

Fix:
  {specific changes needed}

Tests needed:
  {what tests would prevent this regression}
```

## Things NOT to do

- Do not randomly modify code hoping to fix the issue
- Do not skip layers in the trace — the problem may be earlier than expected
- Do not ignore `NotImplementedException` — many handlers are skeletons
- Do not assume the database is running — check connection configuration first

## Related skills

- `/add-tests` — add regression tests after fixing
- `/observability-review` — improve logging if troubleshooting was difficult
- `/code-review` — review the fix before merging
