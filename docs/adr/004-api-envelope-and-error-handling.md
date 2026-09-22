# ADR-004: API Request/Response Envelope and Error Handling

## Status

Accepted

## Context

The API needs a consistent contract for all requests and responses. Consumers must be able to:

- Pass cross-cutting metadata (correlation ID, request ID, source, channel) with every request
- Receive a predictable response structure for both success and error cases
- Correlate requests across services for observability
- Distinguish between different error categories (validation, domain, not-found, internal)

Additionally, authentication tokens (JWT) must be handled via standard HTTP mechanisms, not embedded in the request payload.

## Decision

All API endpoints use standardized envelope types:

**Request:**
```json
{
  "headers": {
    "correlationId": "...",
    "requestId": "...",
    "source": "...",
    "channel": "..."
  },
  "payload": { ... }
}
```

**Successful Response:**
```json
{
  "payload": { ... },
  "exception": null
}
```

**Error Response:**
```json
{
  "payload": null,
  "exception": {
    "code": "ORDER_NOT_FOUND",
    "message": "Order was not found.",
    "details": []
  }
}
```

**Error mapping:**

| Exception Type | HTTP Status | Error Code |
|----------------|-------------|------------|
| `ValidationException` | 400 | `VALIDATION_ERROR` |
| `NotFoundException` | 404 | `NOT_FOUND` |
| `DomainException` | 422 | `DOMAIN_ERROR` |
| `UnauthorizedAccessException` | 401 | `UNAUTHORIZED` |
| Unhandled | 500 | `INTERNAL_ERROR` |

JWT authentication uses the standard `Authorization: Bearer <token>` HTTP header. It is **not** placed inside the request envelope.

A global `IExceptionHandler` ensures all unhandled exceptions produce the envelope format.

## Consequences

**Benefits:**

- Consistent contract for all consumers — no ambiguity about response shape
- Correlation IDs flow through the entire request pipeline for observability
- Error responses are machine-parseable with structured error codes
- JWT stays in standard HTTP headers — compatible with middleware, API gateways, and tooling

**Risks:**

- Envelope adds a layer of wrapping that increases payload size slightly
- Consumers must understand the envelope convention (documented in README and OpenAPI)

## Alternatives Considered

**RFC 7807 ProblemDetails only:** Considered. ProblemDetails is a standard error format but does not cover the request envelope with correlation headers or the success response wrapper. The custom envelope provides a superset of what ProblemDetails offers.

**JWT in request payload:** Rejected. Breaks standard HTTP authentication middleware, API gateway integration, and security tooling expectations.
