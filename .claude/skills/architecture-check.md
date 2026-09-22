# Architecture Check

Verify dependency rules and module boundaries across the solution.

## When to use

- After creating or modifying project references
- After adding a new module
- After moving types between layers
- As part of pre-PR validation

## Before you start

1. Read `CLAUDE.md` for the dependency rules summary.
2. Read `tests/OrderProcessing.ArchitectureTests/DomainDependencyTests.cs` — the enforced domain rules.
3. Read `tests/OrderProcessing.ArchitectureTests/ApplicationDependencyTests.cs` — the enforced application rules.
4. Read `tests/OrderProcessing.ArchitectureTests/ModuleBoundaryTests.cs` — the enforced module boundary rules.

## Architecture rules

These are the canonical dependency rules from ADR-002. Every violation must be reported.

```
LAYER RULES:
  Domain        → BuildingBlocks.Domain only (NO EF Core, NO ASP.NET Core, NO Refit)
  Contracts     → BuildingBlocks.Contracts only
  Application   → Domain + Contracts + BuildingBlocks.Application (NO EF Core, NO Refit)
  Infrastructure→ Application + Domain + BuildingBlocks.Infrastructure (EF Core, Refit allowed)
  API           → Infrastructure projects + BuildingBlocks.Application + BuildingBlocks.Infrastructure

MODULE BOUNDARY RULES:
  {Module}.Domain must NOT reference any other module's Domain, Application, or Infrastructure
  {Module}.Application must NOT reference any other module's Application or Infrastructure
  {Module}.Infrastructure must NOT reference any other module's Infrastructure
  Cross-module communication ONLY through Contracts projects
```

## Steps

1. **Static analysis — inspect `.csproj` files.** For each module under `src/Modules/`, read all four `.csproj` files and verify `<ProjectReference>` entries match the allowed dependencies above. Report any violations.

2. **Check Domain projects.** Verify they reference ONLY `BuildingBlocks.Domain`. Check they have NO `<PackageReference>` to `Microsoft.EntityFrameworkCore`, `Refit`, or `Microsoft.AspNetCore.*` (MediatR is allowed via `BuildingBlocks.Domain` → `IDomainEvent`).

3. **Check Application projects.** Verify they reference their module's Domain, Contracts, and `BuildingBlocks.Application`. Verify NO `<PackageReference>` to `Microsoft.EntityFrameworkCore` or `Refit`. Allowed packages: `MediatR`, `FluentValidation`, `Microsoft.Extensions.DependencyInjection.Abstractions`, `Microsoft.Extensions.Logging.Abstractions`.

4. **Check Contracts projects.** Verify they reference ONLY `BuildingBlocks.Contracts`. No other dependencies.

5. **Check Infrastructure projects.** Verify they reference their module's Application, Domain, and `BuildingBlocks.Infrastructure`. Verify they do NOT reference other modules' Domain/Application/Infrastructure.

6. **Check API project.** Read `src/OrderProcessing.Api/OrderProcessing.Api.csproj`. Verify it references only Infrastructure projects (not Domain or Application directly, except BuildingBlocks).

7. **Run the build.** Execute `dotnet build` and report any compilation errors.

8. **Run architecture tests.** Execute `dotnet test --filter "FullyQualifiedName~ArchitectureTests"` and report results.

9. **Check that all modules are covered by architecture tests.** Verify that `DomainDependencyTests.DomainAssemblies` includes all domain assemblies. Verify `ApplicationDependencyTests.ApplicationAssemblies` includes all application assemblies. Verify `ModuleBoundaryTests` covers cross-module isolation.

## Checklist

- [ ] All `.csproj` references follow allowed dependency graph
- [ ] No Domain project references EF Core, ASP.NET Core, or Refit
- [ ] No Application project references EF Core or Refit
- [ ] No cross-module references outside Contracts
- [ ] `dotnet build` succeeds with zero warnings
- [ ] `dotnet test --filter "FullyQualifiedName~ArchitectureTests"` passes
- [ ] All modules are included in architecture test assemblies

## Output

Report findings as:

```
Architecture Check Results
──────────────────────────
Build:              PASS/FAIL
Architecture Tests: PASS/FAIL
Static Analysis:    PASS/FAIL

Violations:
  [LAYER]  {file} references {forbidden dependency} — {explanation}
  [MODULE] {file} references {other module} — use Contracts for cross-module communication

Warnings:
  [COVERAGE] {module} not included in {test class} — add assembly to TheoryData
```

Do NOT automatically fix violations. Explain each one so the developer understands the architectural constraint.

## Related skills

- `/new-module` — after creating a module, verify architecture
- `/pre-pr` — architecture check is part of PR readiness
- `/code-review` — architecture is a review dimension
- `/refactor` — verify architecture after structural changes
