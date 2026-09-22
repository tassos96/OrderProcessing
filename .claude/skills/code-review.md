# Code Review

Structured review of current changes against architecture, .NET conventions, security, observability, and testing.

## When to use

- Before opening a pull request
- Reviewing another developer's changes
- After completing a significant implementation

## Before you start

1. Run `git diff --stat` to identify changed files.
2. Run `git diff` (or `git diff --cached` for staged changes) to see the actual changes.
3. Read `CLAUDE.md` for architecture rules and conventions.
4. Run `dotnet build` to verify the code compiles.

## Steps

### 1. Classify changed files

For each changed file, determine its layer and module:
- **Domain** — `src/Modules/{Module}/{Module}.Domain/`
- **Application** — `src/Modules/{Module}/{Module}.Application/`
- **Infrastructure** — `src/Modules/{Module}/{Module}.Infrastructure/`
- **API** — `src/OrderProcessing.Api/`
- **Contracts** — `src/Modules/{Module}/{Module}.Contracts/`
- **BuildingBlocks** — `src/BuildingBlocks/`
- **Tests** — `tests/`
- **Config** — `appsettings.json`, `.csproj`, `Directory.Packages.props`

### 2. Architecture review

Check for dependency direction violations:
- Domain references Infrastructure or API packages?
- Application references EF Core or Refit?
- Controller contains business logic beyond `ISender.Send()` delegation?
- Cross-module references outside of Contracts?
- Infrastructure types exposed to Application layer?

### 3. .NET conventions review

- File-scoped namespaces (`namespace Foo;`)
- Primary constructors used where appropriate (handlers, controllers, repos, middleware)
- `sealed` on all concrete classes
- `async/await` used correctly (no `.Result` or `.Wait()`)
- `CancellationToken` passed through entire chain
- Nullable reference types respected (no `!` suppression without justification)
- DTOs and events are `sealed record` types
- No unused `using` statements

### 4. Security review

- No secrets in code or configuration files
- No JWT tokens, passwords, or PII in log statements
- Authorization attributes present on new endpoints
- Input validated via FluentValidation validators
- Error responses use `ResponseEnvelope<T>.Failure()` — no stack traces exposed
- External service calls use the port/adapter pattern (not raw `HttpClient`)

### 5. Observability review

- Structured logging used (`{PropertyName}` not string interpolation in log templates)
- Appropriate log levels (Information for normal flow, Warning for recoverable issues, Error for failures)
- No sensitive data in log messages
- CorrelationId available in the request context

### 6. Testing review

- New public behavior has at least one test
- Test naming follows `MethodName_Condition_ExpectedBehavior`
- Tests use FluentAssertions (not raw `Assert.*`)
- Domain tests exercise entities directly (no mocking)
- Architecture tests updated if new assemblies were added

### 7. Produce review summary

Classify each finding with severity:

| Severity | Meaning |
|---|---|
| **CRITICAL** | Architecture violation, security vulnerability, data loss risk |
| **HIGH** | Missing tests for new behavior, incorrect error handling, broken convention |
| **MEDIUM** | Suboptimal pattern, missing validation, incomplete logging |
| **LOW** | Style inconsistency, minor naming issue |
| **SUGGESTION** | Optional improvement, not a defect |

## Output format

```
Code Review Summary
═══════════════════
Files reviewed: {count}
Modules affected: {list}

Architecture:     {status}
Security:         {status}
Reliability:      {status}
Testing:          {status}
Maintainability:  {status}

Findings:
  [{severity}] {file}:{line} — {description}
  ...

Blocking issues: {count}
Non-blocking issues: {count}

Recommended actions:
  1. {action}
  ...
```

## Things NOT to do

- Do not invent issues — every finding must reference the actual diff
- Do not automatically modify code during review unless explicitly requested
- Do not flag skeleton TODOs that were already present before the change
- Do not require 100% test coverage — focus on meaningful coverage of new behavior

## Related skills

- `/architecture-check` — deeper architecture verification
- `/security-review` — dedicated security assessment
- `/observability-review` — dedicated observability assessment
- `/pre-pr` — complete PR readiness checklist
