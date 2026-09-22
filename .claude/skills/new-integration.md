# New External Integration

Add an external API integration following the Ports & Adapters pattern (ADR-003).

## When to use

- Integrating with a new external service (e.g., Fraud Detection API, Email Provider, Tax Service)
- Adding a new external dependency to an existing module

## Before you start

1. Read ADR-003: `docs/adr/003-external-integration-strategy.md`
2. Read the canonical integration example from the Orders module:
   - **Port (Application):** `src/Modules/Orders/Orders.Application/Abstractions/IInventoryService.cs`
   - **Refit interface (Infrastructure):** `src/Modules/Orders/Orders.Infrastructure/ExternalServices/IInventoryApi.cs`
   - **Adapter (Infrastructure):** `src/Modules/Orders/Orders.Infrastructure/ExternalServices/InventoryServiceClient.cs`
   - **Options (Infrastructure):** `src/Modules/Orders/Orders.Infrastructure/Options/ExternalServiceOptions.cs`
   - **DI registration:** `src/Modules/Orders/Orders.Infrastructure/Extensions/ServiceCollectionExtensions.cs`
3. Read `src/OrderProcessing.Api/appsettings.json` for the configuration structure.

## Steps

1. **Define the port interface** in `src/Modules/{Module}/{Module}.Application/Abstractions/I{Service}Service.cs`:
   - Define operations the application layer needs (e.g., `Task<FraudCheckResult> CheckAsync(...)`)
   - Include `CancellationToken cancellationToken = default` on all async methods
   - Define result types as records in the same file if they're specific to this service:
     ```
     public sealed record FraudCheckResult(bool IsApproved, string? RejectionReason);
     ```
   - This interface is the ONLY thing the Application layer knows about the external service

2. **Create the Refit interface** in `src/Modules/{Module}/{Module}.Infrastructure/ExternalServices/I{Service}Api.cs`:
   - Decorate with `[Headers("Content-Type: application/json")]`
   - Define HTTP endpoints matching the external API:
     ```
     [Post("/api/fraud/check")]
     Task<ApiResponse<FraudCheckResponse>> CheckAsync([Body] FraudCheckRequest request, CancellationToken cancellationToken = default);
     ```
   - Define request/response DTOs as `sealed record` types in the same file
   - These DTOs match the external API shape — they may differ from the Application-layer result types

3. **Create the adapter** in `src/Modules/{Module}/{Module}.Infrastructure/ExternalServices/{Service}Client.cs`:
   - `sealed class` with primary constructor injecting the Refit interface
   - Implements the Application-layer port interface
   - Maps between Refit DTOs and Application-layer result types
   - For skeleton: `throw new NotImplementedException();` with TODO comments:
     ```
     // TODO: Call {service}Api.{Method}(), map response to Application-layer result
     // TODO: Handle API errors (non-success status codes, network failures)
     // TODO: Consider retry/timeout/circuit-breaker (see ADR-005)
     ```

4. **Create the options class** in `src/Modules/{Module}/{Module}.Infrastructure/Options/{Service}ApiOptions.cs`:
   ```
   public sealed class {Service}ApiOptions
   {
       public const string SectionName = "ExternalServices:{ServiceName}";
       public string BaseUrl { get; init; } = default!;
       public int TimeoutSeconds { get; init; } = 30;
   }
   ```

5. **Register in DI** in `src/Modules/{Module}/{Module}.Infrastructure/Extensions/ServiceCollectionExtensions.cs`:
   ```
   var {service}Options = new {Service}ApiOptions();
   configuration.GetSection({Service}ApiOptions.SectionName).Bind({service}Options);

   services.AddRefitClient<I{Service}Api>()
       .ConfigureHttpClient(c => c.BaseAddress = new Uri({service}Options.BaseUrl ?? "https://localhost"));

   services.AddScoped<I{Service}Service, {Service}Client>();
   ```

6. **Add configuration** to `src/OrderProcessing.Api/appsettings.json`:
   ```json
   "ExternalServices": {
       "{ServiceName}": {
           "BaseUrl": "https://{service}-api.example.com",
           "TimeoutSeconds": 30
       }
   }
   ```

7. **Add Refit packages** to the Infrastructure `.csproj` if not already present:
   - `Refit` and `Refit.HttpClientFactory` (no version — central package management)

## Architecture rules

```
Application layer:
  - Defines I{Service}Service (port interface)
  - Knows NOTHING about Refit, HTTP, or external API shapes
  - Tests mock the port interface

Infrastructure layer:
  - Defines I{Service}Api (Refit interface)
  - Implements {Service}Client (adapter)
  - Maps between Refit DTOs and Application-layer types
  - Registers Refit client and adapter in DI

Domain layer:
  - Has ZERO knowledge of external services
```

## Checklist

- [ ] Port interface defined in Application layer with no Infrastructure dependencies
- [ ] Refit interface defined in Infrastructure with `[Headers]` and endpoint attributes
- [ ] Adapter implements the port interface and injects the Refit interface
- [ ] Options class follows `SectionName` convention
- [ ] DI registration adds Refit client and service adapter
- [ ] Configuration added to `appsettings.json`
- [ ] Application `.csproj` has NO reference to Refit
- [ ] Domain `.csproj` has NO reference to Refit
- [ ] `dotnet build` succeeds

## Things NOT to do

- Do not add Refit references to Domain or Application projects
- Do not implement resilience policies (retry, circuit breaker) unless the repository already has `Microsoft.Extensions.Http.Resilience` or Polly configured — add TODOs instead
- Do not call external APIs directly from handlers — always go through the port interface
- Do not log sensitive request/response data (API keys, tokens, PII)

## Related skills

- `/add-tests` — mock the port interface for handler tests
- `/security-review` — verify no secrets or sensitive data exposure
- `/architecture-check` — verify Refit doesn't leak into Domain/Application
- `/observability-review` — ensure external calls are logged with correlation IDs
