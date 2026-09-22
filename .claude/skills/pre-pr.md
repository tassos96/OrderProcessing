# Pre-PR Check

Final checklist before opening a pull request.

## When to use

- About to open a pull request
- Final verification after completing a feature or fix
- Ensuring all quality gates pass

## Before you start

1. Run `git diff --stat` to review what files changed.
2. Ensure all changes are committed or staged.

## Steps

### 1. Build

```bash
dotnet restore
dotnet build
```
Both must succeed with **zero warnings** (TreatWarningsAsErrors is enabled).

### 2. Tests

```bash
dotnet test
```
All tests must pass. If any test is skipped, note the reason.

For focused verification:
```bash
dotnet test --filter "FullyQualifiedName~ArchitectureTests"   # Dependency rules
dotnet test --filter "FullyQualifiedName~ContractTests"       # API contracts
dotnet test --filter "FullyQualifiedName~UnitTests"           # Domain logic
dotnet test --filter "FullyQualifiedName~IntegrationTests"    # API endpoints
```

### 3. Architecture

Run architecture tests and verify they cover all modules:
```bash
dotnet test --filter "FullyQualifiedName~ArchitectureTests"
```
If new modules or assemblies were added, verify they are included in the architecture test `TheoryData`.

### 4. Code hygiene

Review `git diff` for:
- [ ] No new `TODO` comments without a tracking reference (existing skeleton TODOs are acceptable)
- [ ] No `Console.WriteLine`, `Debug.WriteLine`, `Debugger.Break()`, or `Debugger.Launch()`
- [ ] No `Thread.Sleep` or busy waits
- [ ] No commented-out code blocks
- [ ] No hardcoded secrets, API keys, or credentials
- [ ] No `// HACK` or `// FIXME` without explanation

### 5. Security

- [ ] No secrets in `appsettings.json` changes
- [ ] New endpoints have `[Authorize]` attributes
- [ ] No PII or sensitive data in log statements
- [ ] Error responses use `ResponseEnvelope<T>.Failure()` — no stack traces

### 6. Observability

- [ ] New log statements use structured logging (named parameters, not interpolation)
- [ ] Log levels are appropriate (Information/Warning/Error)
- [ ] No sensitive data in log messages

### 7. Tests for changed behavior

- [ ] New domain rules have unit tests
- [ ] New endpoints have integration tests (or skipped with documented reason)
- [ ] New modules are included in architecture tests
- [ ] New integration events are included in contract tests

### 8. API contract changes

- [ ] No breaking changes without documentation
- [ ] `[ProducesResponseType]` attributes match actual responses
- [ ] `RequestEnvelope<T>` / `ResponseEnvelope<T>` used consistently

### 9. Documentation

- [ ] ADR created for significant architectural decisions
- [ ] `CLAUDE.md` updated if architecture or conventions changed
- [ ] Relevant skill files updated if workflow changed

## Output format

```
PR Readiness Report
═══════════════════
Build:           PASS / FAIL
Tests:           PASS / FAIL ({passed}/{total}, {skipped} skipped)
Architecture:    PASS / FAIL
Security:        PASS / FAIL
Observability:   PASS / FAIL
Documentation:   PASS / FAIL / N/A

Blocking issues:
  - {issue 1}
  - {issue 2}

Non-blocking notes:
  - {note 1}

Verdict: READY / NOT READY
```

Only mark a category as `PASS` if it was actually checked. If a category cannot be verified (e.g., no changes in that area), mark it `N/A`.

## Things NOT to do

- Do not mark PASS without actually running the checks
- Do not silently skip failing tests
- Do not auto-fix issues during the PR check — report them for the developer to address
- Do not push to remote unless the developer explicitly asks

## Related skills

- `/code-review` — detailed review of changes
- `/architecture-check` — focused architecture verification
- `/security-review` — focused security assessment
- `/add-tests` — if test coverage is insufficient
