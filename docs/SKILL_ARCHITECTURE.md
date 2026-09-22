# Skill Architecture

Why the developer skills exist and how they enforce the platform's architecture.

## The Problem

A well-designed architecture is only as good as the team's ability to follow it consistently. Without guardrails:

- New developers add code in the wrong layer because they don't know the conventions
- Business logic leaks into controllers or repositories
- Cross-module dependencies creep in because the right pattern isn't obvious
- Security and observability practices are inconsistent
- Code reviews catch violations late, requiring rework

## The Solution

Developer skills are AI-assisted workflows that encode architectural knowledge into repeatable processes. They sit between the architecture (documented in ADRs and CLAUDE.md) and the development team:

```
Architecture (ADRs, CLAUDE.md)
     ↓
Architecture Rules (dependency direction, module boundaries)
     ↓
Developer Skills (guided workflows with built-in verification)
     ↓
Automated Checks (architecture tests, build, contract tests)
     ↓
Code Review (structured review against architecture)
     ↓
Pull Request (verified and documented)
```

## How Skills Enforce Architecture

### Dependency Direction

Every code-modifying skill embeds the dependency rules from ADR-002:

```
Domain        → BuildingBlocks.Domain only
Application   → Domain + Contracts + BuildingBlocks.Application
Infrastructure→ Application + Domain + BuildingBlocks.Infrastructure
API           → Infrastructure projects (composition root)
```

Skills like `/new-module` create projects with the correct references. Skills like `/new-endpoint` place code in the correct layer. Skills like `/architecture-check` verify the rules after changes.

### Module Boundaries

The `/new-module` skill scaffolds modules with isolated projects. The `/architecture-check` skill verifies that no module reaches into another's Domain or Infrastructure. The `/new-integration` skill ensures external APIs are abstracted behind ports in the Application layer.

### Convention Consistency

Skills reference exemplar files (e.g., `OrdersController.cs`, `Order.cs`, `OrdersDbContext.cs`) rather than embedding patterns. This means:

- Skills automatically adapt when conventions evolve
- New developers produce code that matches existing patterns
- The Orders module serves as the living template

### Test Coverage

The `/add-tests` skill determines the correct test type (unit, integration, architecture, contract) based on what changed. The `/new-module` skill automatically adds architecture test coverage. The `/pre-pr` skill verifies all tests pass before PR.

## How Skills Reduce Developer Mistakes

| Common mistake | Prevented by |
|---|---|
| EF Core in Domain layer | `/architecture-check`, `/new-module` |
| Business logic in controller | `/new-endpoint`, `/code-review`, `/refactor` |
| Missing validation | `/new-endpoint`, `/code-review` |
| Secrets in config | `/security-review`, `/pre-pr` |
| Missing authorization | `/security-review`, `/new-endpoint` |
| Cross-module dependency | `/architecture-check`, `/new-module` |
| Missing tests | `/add-tests`, `/pre-pr` |
| Breaking API change | `/api-contract`, `/code-review` |
| Wrong architectural layer | `/new-business-rule`, `/refactor` |
| Missing authorization | `/new-endpoint`, `/security-review`, `/code-review` |
| Wrong auth policy type | `/security-review`, `/new-endpoint` |

## How Skills Improve Consistency

All skills follow the same workflow: **Inspect → Understand → Plan → Modify → Test → Verify**. This means:

- Every code change starts with reading existing patterns
- Every modification is validated against architecture rules
- Every change ends with compilation and test verification
- Every skill recommends related skills for the next step

## How Skills Improve Onboarding

A new developer can:

1. Run `/onboarding` to understand the architecture
2. Read the ADRs for architectural context
3. Use `/new-endpoint` to add their first feature, guided by the skill
4. Run `/pre-pr` to verify their work meets quality standards

The skills encode tribal knowledge that would otherwise take weeks to learn through code review feedback.

## How Skills Support Team Scaling

As the team grows:

- **Consistency** — All developers follow the same patterns regardless of experience
- **Code review efficiency** — Reviewers can focus on business logic, not architecture violations
- **Reduced rework** — Issues caught at development time, not review time
- **Self-service** — Developers can scaffold modules and endpoints without waiting for senior guidance
- **Architecture evolution** — Update the exemplar files and skills automatically adapt

## Skill Composability

Skills are designed to chain together. Common workflows:

```
New feature:      /new-module → /new-endpoint → /new-business-rule → /add-tests → /pre-pr
Integration:      /new-integration → /add-tests → /security-review → /architecture-check → /pre-pr
Bug fix:          /troubleshoot → /add-tests → /code-review → /pre-pr
Refactoring:      /refactor → /architecture-check → /add-tests → /pre-pr
```

No skill creates a circular dependency. The flow is always: scaffold → implement → test → verify → review.

## Maintenance

When the architecture evolves:

1. Update the ADRs in `docs/adr/` first (decisions drive everything)
2. Update `CLAUDE.md` if conventions change
3. Update the exemplar files (the skills reference these by path)
4. Skills that reference exemplar files automatically pick up changes
5. Skills that embed specific rules (e.g., dependency lists) may need manual updates

The skills are lightweight by design — they contain instructions and references, not code templates. This makes them easy to maintain and resistant to staleness.
