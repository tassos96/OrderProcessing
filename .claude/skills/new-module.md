# New Module

Scaffold a new module following the established modular monolith conventions.

## When to use

- Adding a new business capability to the platform (e.g., Discounts, Notifications, Returns)
- The capability requires its own domain, application, infrastructure, and contracts layers

## Before you start

1. Read the Orders module as the canonical template:
   - `src/Modules/Orders/Orders.Domain/Orders.Domain.csproj`
   - `src/Modules/Orders/Orders.Application/Orders.Application.csproj`
   - `src/Modules/Orders/Orders.Infrastructure/Orders.Infrastructure.csproj`
   - `src/Modules/Orders/Orders.Contracts/Orders.Contracts.csproj`
2. Read `src/Modules/Orders/Orders.Infrastructure/Extensions/ServiceCollectionExtensions.cs` for the DI registration pattern.
3. Read `src/Modules/Orders/Orders.Application/Extensions/ServiceCollectionExtensions.cs` for the application DI pattern.
4. Read `src/Modules/Orders/Orders.Infrastructure/Persistence/OrdersDbContext.cs` for the DbContext pattern.
5. Read `src/OrderProcessing.Api/Program.cs` for module registration.
6. Read `OrderProcessing.slnx` for solution structure.
7. Read `src/OrderProcessing.Api/appsettings.json` for connection string pattern.

## Steps

1. **Create the four projects** under `src/Modules/{Name}/`:

   **{Name}.Domain.csproj** — reference only `BuildingBlocks.Domain`. Add `InternalsVisibleTo` for `OrderProcessing.UnitTests`.

   **{Name}.Contracts.csproj** — reference only `BuildingBlocks.Contracts`.

   **{Name}.Application.csproj** — reference `{Name}.Domain`, `{Name}.Contracts`, `BuildingBlocks.Application`. Add packages: `FluentValidation`, `FluentValidation.DependencyInjectionExtensions`, `MediatR`, `Microsoft.Extensions.DependencyInjection.Abstractions`, `Microsoft.Extensions.Logging.Abstractions`.

   **{Name}.Infrastructure.csproj** — reference `{Name}.Application`, `{Name}.Domain`, `BuildingBlocks.Infrastructure`. Add packages: `Microsoft.EntityFrameworkCore.SqlServer`, `Microsoft.Extensions.Options.ConfigurationExtensions`.

2. **Create Domain layer skeleton:**
   - `{Name}.Domain/Entities/` — one placeholder aggregate root with factory method and private constructor
   - `{Name}.Domain/ValueObjects/` — placeholder for future value objects
   - `{Name}.Domain/Events/` — placeholder for future domain events
   - `{Name}.Domain/Repositories/I{Entity}Repository.cs` — repository interface with `GetByIdAsync`, `AddAsync`, `Update`
   - `{Name}.Domain/Exceptions/{Name}DomainException.cs` — module-specific domain exception inheriting `DomainException`

3. **Create Contracts layer skeleton:**
   - `{Name}.Contracts/IntegrationEvents/` — empty directory for future integration events

4. **Create Application layer skeleton:**
   - `{Name}.Application/Extensions/ServiceCollectionExtensions.cs` — `Add{Name}Application()` registering MediatR and FluentValidation from assembly
   - `{Name}.Application/DTOs/` — placeholder DTO
   - `{Name}.Application/Commands/` — placeholder command folder with command record, handler (`throw new NotImplementedException()`), and validator

5. **Create Infrastructure layer skeleton:**
   - `{Name}.Infrastructure/Persistence/{Name}DbContext.cs` — inheriting `BaseDbContext`, using schema `"{name_lowercase}"`, with `ApplyConfigurationsFromAssembly`
   - `{Name}.Infrastructure/Persistence/Configurations/` — placeholder for entity configurations
   - `{Name}.Infrastructure/Persistence/Repositories/{Entity}Repository.cs` — implementing the domain interface
   - `{Name}.Infrastructure/Extensions/ServiceCollectionExtensions.cs` — `Add{Name}Module(IConfiguration configuration)` calling `Add{Name}Application()`, registering DbContext, repository, and `IUnitOfWork`

6. **Register the module:**
   - Add `builder.Services.Add{Name}Module(builder.Configuration);` to `src/OrderProcessing.Api/Program.cs`
   - Add the `using {Name}.Infrastructure.Extensions;` import
   - Add connection string `"{Name}Db"` to `appsettings.json`

7. **Add to the solution file:**
   - Add all four projects to `OrderProcessing.slnx` under a new `/src/Modules/{Name}/` folder
   - Add project references to `src/OrderProcessing.Api/OrderProcessing.Api.csproj` for the Infrastructure project

8. **Update architecture tests:**
   - Add domain assembly to `DomainDependencyTests.DomainAssemblies` in `tests/OrderProcessing.ArchitectureTests/DomainDependencyTests.cs`
   - Add application assembly to `ApplicationDependencyTests.ApplicationAssemblies` in `tests/OrderProcessing.ArchitectureTests/ApplicationDependencyTests.cs`
   - Add module boundary tests to `ModuleBoundaryTests.cs`: `{Name}Domain` must not reference other modules' Domain/Infrastructure

9. **Verify:** Run `dotnet build` and `dotnet test --filter "FullyQualifiedName~ArchitectureTests"`.

## Architecture rules

```
{Name}.Domain         → BuildingBlocks.Domain ONLY
{Name}.Contracts      → BuildingBlocks.Contracts ONLY
{Name}.Application    → {Name}.Domain + {Name}.Contracts + BuildingBlocks.Application
{Name}.Infrastructure → {Name}.Application + {Name}.Domain + BuildingBlocks.Infrastructure
```

## Checklist

- [ ] Four projects created with correct references
- [ ] DbContext uses unique schema name (`"{name_lowercase}"`)
- [ ] Connection string added to `appsettings.json`
- [ ] Module registered in `Program.cs`
- [ ] Projects added to `OrderProcessing.slnx`
- [ ] API project references the new Infrastructure project
- [ ] Architecture tests updated with new assemblies
- [ ] `dotnet build` succeeds
- [ ] Architecture tests pass

## Things NOT to do

- Do not generate full business logic — use `throw new NotImplementedException()` and TODOs
- Do not add Refit clients unless the module needs external integrations (use `/new-integration` for that)
- Do not create unnecessary abstractions
- Do not reference other modules' Domain/Application/Infrastructure

## Related skills

- `/new-endpoint` — add endpoints after module scaffold
- `/database-change` — add entities and EF configurations
- `/architecture-check` — verify the module follows dependency rules
- `/add-tests` — add tests for the new module
