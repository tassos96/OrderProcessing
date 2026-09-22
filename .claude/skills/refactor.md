# Refactor

Safe refactoring that preserves architecture and behavior.

## When to use

- A handler is too large and needs extraction
- Business logic is in the wrong layer (e.g., controller contains domain logic)
- Code duplication across handlers or modules
- Improving naming or structure without changing behavior

## Before you start

1. Run `dotnet test` to establish a green baseline. If tests fail, fix them first — do not refactor broken code.
2. Read the target code completely. Understand what it does and which layer it belongs to.
3. Identify all tests that cover the target behavior.
4. Determine the architectural problem:
   - Is logic in the wrong layer?
   - Is a class doing too much?
   - Is there duplication that should be extracted?
   - Are dependencies pointing the wrong direction?

## Steps

1. **Understand before changing.** Read the code, the tests, and the callers. State what the code currently does and what the problem is.

2. **Identify the smallest safe refactor.** Do not combine multiple refactorings. Examples:
   - Extract a method from a handler
   - Move validation logic from a handler to a validator
   - Move domain logic from a handler to the entity
   - Extract a shared abstraction into BuildingBlocks
   - Rename a type and update all references

3. **Verify architectural correctness** of the proposed change:
   - Will the refactored code still follow the dependency direction?
   - Will Domain remain free of Infrastructure references?
   - Will the controller remain a thin delegation layer?
   - Will module boundaries be preserved?

4. **Implement incrementally.** Make one change at a time:
   - Move or extract the code
   - Update namespaces and imports
   - Update DI registration if services moved
   - Update project references if types moved between projects

5. **After each step, run:**
   ```bash
   dotnet build
   dotnet test
   ```

6. **After structural changes, run architecture tests:**
   ```bash
   dotnet test --filter "FullyQualifiedName~ArchitectureTests"
   ```

7. **Verify no behavioral change.** All existing tests should still pass without modification. If tests need updating, the refactoring may be changing behavior — reconsider.

## Architecture rules

When moving code between layers, the dependency direction must be preserved:

| Moving FROM | Can move TO | Cannot move TO |
|---|---|---|
| Controller | Application handler | Domain |
| Handler | Domain entity, Application validator | Controller, Infrastructure |
| Infrastructure | (rarely moves) | Domain, Application |
| Domain entity | (rarely moves out) | Application, Infrastructure |

Common refactoring patterns:
- **Controller logic → Handler:** Extract `ISender.Send()` call, move logic to handler
- **Handler logic → Entity:** Move domain invariants into entity methods
- **Duplicated logic → BuildingBlocks:** Extract shared types to BuildingBlocks (appropriate layer)

## Checklist

- [ ] Green test baseline established before refactoring
- [ ] Smallest safe refactor identified
- [ ] All existing tests pass after refactoring (without test modifications)
- [ ] Architecture tests pass
- [ ] `dotnet build` succeeds with zero warnings
- [ ] Dependency direction preserved
- [ ] No behavioral change introduced

## Things NOT to do

- Do not refactor and add features in the same change
- Do not perform large rewrites — prefer incremental changes
- Do not introduce new abstractions or patterns not already in the codebase
- Do not refactor without a green test baseline
- Do not modify tests to make them pass after refactoring (that's a behavior change, not a refactoring)

## Related skills

- `/architecture-check` — verify architecture after structural changes
- `/add-tests` — add tests before refactoring if coverage is insufficient
- `/code-review` — review may identify refactoring needs
- `/pre-pr` — ensure refactoring doesn't break anything
