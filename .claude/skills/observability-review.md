# Observability Review

Review logging, correlation, and tracing readiness across the application.

## When to use

- After adding new endpoints or handlers
- After modifying logging configuration
- When assessing production readiness
- When troubleshooting is difficult due to poor observability

## Before you start

1. Read `docs/architecture/logging-strategy.md` for the logging standards.
2. Read `src/OrderProcessing.Api/Program.cs` for Serilog and middleware setup.
3. Read `src/OrderProcessing.Api/Middleware/CorrelationIdMiddleware.cs` for correlation ID propagation.
4. Read `src/OrderProcessing.Api/Middleware/GlobalExceptionHandler.cs` for error logging.
5. Read `src/OrderProcessing.Api/appsettings.json` section `"Serilog"` for log configuration.

## Steps

### 1. Serilog configuration

Verify in `Program.cs`:
- Bootstrap logger: `new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger()` exists for startup errors
- Full configuration: `builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration))`
- Request logging: `app.UseSerilogRequestLogging()` is in the middleware pipeline

Verify in `appsettings.json`:
- Minimum levels set: `Default: Information`, overrides for `Microsoft.AspNetCore: Warning` and `Microsoft.EntityFrameworkCore: Warning`
- Enrichers configured: `FromLogContext`, `WithMachineName`, `WithThreadId`

### 2. Correlation ID flow

Verify the full correlation flow:
```
Client → X-Correlation-Id header
  → CorrelationIdMiddleware reads or generates ID
  → LogContext.PushProperty("CorrelationId", correlationId)
  → Response header X-Correlation-Id set
  → All log entries within the request include CorrelationId
```

Check that `CorrelationIdMiddleware` is registered BEFORE `UseSerilogRequestLogging()` in the pipeline.

### 3. Structured logging

Search for log statements in handlers, services, and middleware:
- Verify named parameters: `Log.Information("Order {OrderId} created", orderId)` ✓
- Flag string interpolation: `Log.Information($"Order {orderId} created")` ✗
- Check that log templates are meaningful (not just "Entering method" / "Exiting method")

### 4. Contextual properties

Check that handlers push relevant context to `LogContext`:
- `OrderId` when processing an order
- `CustomerId` when handling customer-scoped operations
- `ModuleName` in module-level operations

These enable filtering logs by business entity across the entire request pipeline.

### 5. Log levels

Verify appropriate use per the logging strategy:
| Level | Correct usage |
|---|---|
| `Debug` | Detailed diagnostics (development only) |
| `Information` | Normal flow: request received, order created, payment processed |
| `Warning` | Recoverable issues: retry triggered, validation failed, external service slow |
| `Error` | Unhandled exceptions, external service failures, data inconsistencies |
| `Fatal` | Application startup failures |

### 6. Sensitive data

Reference `docs/architecture/logging-strategy.md` sensitive data policy. Check that logs do NOT contain:
- JWT tokens or bearer tokens
- Passwords or password hashes
- Payment card numbers, CVVs
- API keys or secrets
- Unnecessary PII

### 7. Health checks

Verify health check endpoints in `Program.cs`:
- `/health/live` — liveness probe (always returns 200 if process is running)
- `/health/ready` — readiness probe (checks database connectivity and external services)
- Check for TODO items indicating missing health check registrations

### 8. Tracing readiness

Assess OpenTelemetry readiness (currently a future enhancement per `docs/architecture/logging-strategy.md`):
- Is `Activity.Current?.Id` available for distributed tracing?
- Are external HTTP calls traceable via `HttpClient` instrumentation?
- Document what would be needed to add OpenTelemetry

### 9. Troubleshooting flow

Explain how an engineer can trace a request end-to-end:

```
1. Client sends request with X-Correlation-Id header
2. CorrelationIdMiddleware captures/generates ID → pushed to LogContext
3. SerilogRequestLogging logs: HTTP method, path, status, elapsed time, CorrelationId
4. ValidationBehavior logs validation results (via LoggingBehavior)
5. Handler logs business operations with OrderId, CustomerId
6. External service calls logged with CorrelationId in context
7. GlobalExceptionHandler logs unhandled exceptions with error code
8. Response includes X-Correlation-Id for client-side correlation
```

## Output format

```
Observability Review Results
════════════════════════════
Serilog Config:     {status}
Correlation ID:     {status}
Structured Logging: {status}
Log Levels:         {status}
Sensitive Data:     {status}
Health Checks:      {status}
Tracing Readiness:  {status}

Findings:
  [{severity}] {file}:{line} — {description}
  ...

Troubleshooting capability: {assessment}
```

## Things NOT to do

- Do not add OpenTelemetry or distributed tracing infrastructure unless explicitly requested
- Do not modify log statements during review unless requested
- Do not claim tracing is complete if only logging is configured

## Related skills

- `/security-review` — overlaps with sensitive data in logs
- `/code-review` — observability is a review dimension
- `/troubleshoot` — observability enables effective troubleshooting
- `/pre-pr` — observability check is part of PR readiness
