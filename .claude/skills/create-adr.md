# Create ADR

Create an Architecture Decision Record following the established format.

## When to use

- Making a significant architectural decision (new technology, pattern change, integration approach)
- Documenting a trade-off that future developers need to understand
- Recording why an alternative was rejected

## Before you start

1. Read existing ADRs in `docs/adr/` to understand the format and numbering:
   - `docs/adr/001-modular-monolith-architecture.md`
   - `docs/adr/002-clean-architecture-and-dependency-direction.md`
   - `docs/adr/003-external-integration-strategy.md`
   - `docs/adr/004-api-envelope-and-error-handling.md`
   - `docs/adr/005-order-consistency-and-reliability.md`
2. Determine the next ADR number by reading the `docs/adr/` directory.

## Steps

1. **Determine the next number.** List files in `docs/adr/` and increment the highest number.

2. **Create the ADR file** at `docs/adr/{NNN}-{slug}.md` where `{slug}` is a lowercase hyphenated summary (e.g., `006-redis-caching-strategy.md`).

3. **Follow the exact format** from existing ADRs:

   ```markdown
   # ADR-{NNN}: {Title}

   ## Status

   Proposed

   ## Context

   {Describe the forces, constraints, and requirements that led to this decision.
   What problem are we solving? What are the constraints? Why now?}

   ## Decision

   {State the decision clearly and specifically.
   Include diagrams, code patterns, or configuration examples where helpful.}

   ## Consequences

   **Benefits:**

   - {Benefit 1}
   - {Benefit 2}

   **Risks:**

   - {Risk 1 (with mitigation)}
   - {Risk 2 (with mitigation)}

   ## Alternatives Considered

   **{Alternative 1}:** Rejected. {Reason — be specific about why this doesn't work for our context.}

   **{Alternative 2}:** Rejected. {Reason.}
   ```

4. **Set status to "Proposed"** — not "Accepted". ADRs should be reviewed before acceptance.

5. **Reference related ADRs** if the decision builds on or modifies a previous one.

## Architecture rules

- ADRs are documentation, not code — they live in `docs/adr/`
- ADRs document decisions, not implementation details
- Each ADR addresses ONE decision (not a collection of related decisions)
- ADRs are immutable once accepted — create a new ADR to supersede an old one

## Checklist

- [ ] File named `docs/adr/{NNN}-{slug}.md` with correct number
- [ ] All sections present: Status, Context, Decision, Consequences, Alternatives Considered
- [ ] Status is "Proposed" (not "Accepted" until reviewed)
- [ ] Context explains WHY the decision is needed
- [ ] Decision is specific and actionable
- [ ] Consequences include both benefits AND risks
- [ ] At least one alternative is documented with rejection reason
- [ ] No trivial decisions (ADRs are for significant architectural choices)

## Things NOT to do

- Do not create ADRs for trivial implementation details (variable naming, minor refactoring)
- Do not set status to "Accepted" without team review
- Do not modify existing accepted ADRs — create a new one that supersedes
- Do not use ADRs as design documents — they record decisions, not designs

## Related skills

- `/onboarding` — ADRs are key onboarding material
- `/code-review` — review should check if ADR is needed for significant changes
