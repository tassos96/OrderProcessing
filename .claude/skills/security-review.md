# Security Review

Assess the codebase for security vulnerabilities and configuration weaknesses.

## When to use

- Before deploying to production
- After adding authentication, authorization, or external integrations
- As part of a comprehensive code review
- After modifying error handling or logging

## Before you start

1. Read `src/OrderProcessing.Api/Program.cs` for authentication and authorization setup.
2. Read `src/OrderProcessing.Api/Options/JwtOptions.cs` for JWT configuration.
3. Read `src/OrderProcessing.Api/Middleware/GlobalExceptionHandler.cs` for error response patterns.
4. Read `src/OrderProcessing.Api/Middleware/CorrelationIdMiddleware.cs` for header handling.
5. Read `docs/architecture/logging-strategy.md` for the sensitive data policy.
6. Read `src/OrderProcessing.Api/appsettings.json` for configuration values.

## Steps

### 1. Authentication

- Verify JWT Bearer is configured in `Program.cs`
- Check `JwtOptions` — `Authority`, `Audience`, `RequireHttpsMetadata` should not be hardcoded to insecure values
- Check that `RequireHttpsMetadata` is `true` for production configurations
- Verify token validation parameters are configured (currently a TODO — flag if still missing)

### 2. Authorization

- Check every controller action for `[Authorize(Policy = "...")]` attribute — **all actions must be protected**
- Verify authorization policies in `Program.cs` use `RequireClaim("permission", "{module}.{action}")` — not just `RequireAuthenticatedUser()`
- Flag any endpoint that modifies state but lacks authorization
- Verify `DevAuthController` is gated to Development environment only (check `IsDevelopment()` guard)
- Verify test user permissions in `DevAuthController` match the policies in `Program.cs`

### 3. Secrets management

- Scan `appsettings.json` and `appsettings.Development.json` for hardcoded secrets
- Check for API keys, passwords, or tokens in code files
- Verify connection strings use placeholder values or reference environment variables
- Check `.gitignore` for sensitive file patterns

### 4. Input validation

- Verify all commands have corresponding `AbstractValidator<T>` classes
- Check that validators cover all user-supplied fields
- Look for unvalidated string inputs that could be used in logging, queries, or responses
- Verify `RequestEnvelope<T>` is used for all POST/PUT endpoints (provides structured input)

### 5. Error handling

- Verify `GlobalExceptionHandler` does not expose stack traces or internal error details in responses
- Check that the generic 500 handler returns `"An unexpected error occurred."` — not the actual exception message
- Verify error codes follow the SCREAMING_SNAKE_CASE convention
- Check that domain exceptions are mapped to 422, not 500

### 6. Logging security

Reference the sensitive data policy from `docs/architecture/logging-strategy.md`:
- Search for log statements containing: JWT tokens, passwords, payment card numbers, CVVs, API keys, PII
- Verify structured logging uses named parameters (`{OrderId}`) not string interpolation that might include sensitive data
- Check that external API responses are not logged in full (they may contain secrets)

### 7. External service security

- Verify Refit interfaces use HTTPS base URLs (not HTTP) in production config
- Check that API keys for external services are not in `appsettings.json`
- Verify external service adapters do not log request/response bodies containing sensitive data
- Check for timeout configuration on external HTTP clients

### 8. SQL injection

- Verify all database access uses EF Core (parameterized by default)
- Search for raw SQL usage (`FromSqlRaw`, `ExecuteSqlRaw`) — if found, verify parameters are used
- Verify no string concatenation in query construction

### 9. Insecure deserialization

- Check that `System.Text.Json` is used (not `BinaryFormatter` or `Newtonsoft.Json` with `TypeNameHandling`)
- Verify no `JsonSerializerOptions` with unsafe type handling

### 10. API boundary

- Verify domain entities are never exposed directly through the API — always use DTOs
- Check that internal IDs or implementation details are not leaked in error responses
- Verify `ResponseEnvelope<T>` is used consistently (no raw objects returned)

## Output format

```
Security Review Results
═══════════════════════
Authentication:    {status} — {summary}
Authorization:     {status} — {summary}
Secrets:           {status} — {summary}
Input Validation:  {status} — {summary}
Error Handling:    {status} — {summary}
Logging:           {status} — {summary}
External Services: {status} — {summary}
SQL Injection:     {status} — {summary}
API Boundary:      {status} — {summary}

Findings:
  [{CRITICAL|HIGH|MEDIUM|LOW}] {file}:{line} — {description}
  ...

Recommended actions:
  1. {action}
  ...
```

## Things NOT to do

- Do not claim compliance with security standards (OWASP, SOC2, etc.) unless actually verified
- Do not introduce security frameworks not already in the repository
- Do not generate false positives — every finding must be specific and actionable
- Do not modify code during review unless explicitly requested

## Related skills

- `/code-review` — security is one dimension of code review
- `/observability-review` — overlaps with logging security
- `/pre-pr` — security check is part of PR readiness
