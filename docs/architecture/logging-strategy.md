# Logging Strategy

## Overview

The Order Processing Platform uses **Serilog** for structured logging. All log entries are machine-parseable JSON with contextual properties that enable correlation, filtering, and analysis.

## Configuration

### Sinks

| Environment | Sink | Purpose |
|-------------|------|---------|
| Development | Console | Human-readable output during local development |
| Staging / Production | Seq, Elasticsearch, or cloud logging service | Centralized log aggregation and search |

### Log Levels

| Level | Usage |
|-------|-------|
| `Debug` | Detailed diagnostic information (development only) |
| `Information` | Normal application flow: request received, order created, payment processed |
| `Warning` | Recoverable issues: retry triggered, validation failed, external service slow |
| `Error` | Unhandled exceptions, external service failures, data inconsistencies |
| `Fatal` | Application startup failures, unrecoverable errors |

Default minimum level: `Information` (production), `Debug` (development).

## Contextual Enrichment

Every log entry is enriched with:

| Property | Source | Purpose |
|----------|--------|---------|
| `CorrelationId` | `X-Correlation-Id` header or auto-generated | Trace a request across all log entries |
| `RequestId` | Request envelope `headers.requestId` | Unique identifier for idempotency and tracing |
| `MachineName` | Serilog enricher | Identify which instance produced the log |
| `ThreadId` | Serilog enricher | Diagnose concurrency issues |

Application-specific properties are added via `LogContext.PushProperty`:

- `OrderId` — when processing an order
- `CustomerId` — when handling a customer-scoped operation
- `ModuleName` — which business module is executing

### Example Structured Log Entry

```json
{
  "Timestamp": "2026-09-22T14:30:00.000Z",
  "Level": "Information",
  "MessageTemplate": "Order {OrderId} created for customer {CustomerId}",
  "Properties": {
    "OrderId": "abc-123",
    "CustomerId": "cust-456",
    "CorrelationId": "corr-789",
    "RequestId": "req-012",
    "MachineName": "web-01"
  }
}
```

## Sensitive Data Policy

The following must **never** appear in logs:

- JWT tokens or bearer tokens
- Passwords or password hashes
- Payment card numbers, CVVs, or payment credentials
- API keys or secrets
- Personal Identifiable Information (PII) beyond what is necessary for debugging

Use Serilog destructuring policies or custom enrichers to mask sensitive fields.

## Request Logging

Serilog's `UseSerilogRequestLogging()` middleware logs every HTTP request with:

- HTTP method and path
- Response status code
- Elapsed time in milliseconds
- Correlation ID

This replaces the default ASP.NET Core request logging with a single structured log entry per request.

## Future Enhancements

### OpenTelemetry

Integrate `OpenTelemetry.Exporter.Console` (development) and `OpenTelemetry.Exporter.Otlp` (production) for:

- **Distributed tracing**: Trace requests across modules and external services
- **Metrics**: Request rate, error rate, latency percentiles, queue depth
- **Baggage**: Propagate correlation IDs across service boundaries

### Centralized Logging

Deploy Seq, ELK (Elasticsearch + Logstash + Kibana), or a cloud-native logging service (Azure Monitor, AWS CloudWatch, Datadog) for:

- Full-text search across all application logs
- Dashboards and alerting on error rates
- Log retention policies
