# Developer Skills Reference

AI-assisted development skills for the Order Processing Platform. Each skill is a guided workflow that inspects the repository, follows established conventions, and enforces architectural rules.

## Quick Reference

| Skill | Purpose | Changes code? |
|---|---|---|
| `/code-review` | Structured review of current changes | No |
| `/new-module` | Scaffold a new module (4 layers + tests) | Yes |
| `/new-endpoint` | Add an API endpoint end-to-end | Yes |
| `/new-business-rule` | Add a business rule in the correct layer | Yes |
| `/add-tests` | Add tests for changed code | Yes |
| `/new-integration` | Add external API integration (Ports & Adapters) | Yes |
| `/database-change` | Add entity, EF configuration, repository | Yes |
| `/architecture-check` | Verify dependency rules and module boundaries | No |
| `/security-review` | Security assessment | No |
| `/observability-review` | Logging, correlation, and tracing review | No |
| `/pre-pr` | Final checklist before opening a PR | No |
| `/troubleshoot` | Systematic debugging following request flow | Maybe |
| `/refactor` | Safe refactoring preserving architecture | Yes |
| `/domain-event` | Add domain event with handler skeleton | Yes |
| `/api-contract` | API contract changes with compatibility analysis | Yes |
| `/onboarding` | Repository walkthrough for new developers | No |
| `/create-adr` | Create an Architecture Decision Record | Yes |

## Example Scenarios

### "I need to add a Discounts module"

```
/new-module         → Scaffold Discounts.Domain, .Application, .Infrastructure, .Contracts
/new-endpoint       → Add POST /api/discounts and GET /api/discounts/{id}
/new-business-rule  → Add "discount cannot exceed 50%" domain rule
/add-tests          → Unit tests for domain, integration tests for endpoints
/architecture-check → Verify dependency rules
/pre-pr             → Final verification before PR
```

### "I need to integrate a Fraud Detection API"

```
/new-integration    → Create IFraudDetectionService port, Refit interface, adapter
/add-tests          → Mock the port interface, test handler integration
/security-review    → Verify no secrets, secure HTTP, input validation
/observability-review → Ensure correlation IDs flow to external calls
/architecture-check → Verify Refit doesn't leak into Domain/Application
/pre-pr             → Final verification
```

### "I need to fix a production bug"

```
/troubleshoot       → Trace the request flow, identify root cause
/add-tests          → Add regression test for the bug
/code-review        → Review the fix
/architecture-check → Verify fix doesn't violate architecture
/pre-pr             → Final verification
```

### "I need to review another developer's PR"

```
/code-review        → Structured review of their changes
/security-review    → Check for security issues
/architecture-check → Verify dependency rules
```

### "I'm new to this repository"

```
/onboarding         → Guided walkthrough of architecture, modules, and conventions
```

Then get a dev token to test endpoints:
1. `dotnet run --project src/OrderProcessing.Api`
2. `POST /api/dev/token` with `{ "username": "admin" }` → get JWT
3. Use `Authorization: Bearer <token>` to call any endpoint

### "I need to add a business rule: orders cannot exceed $10,000"

```
/new-business-rule  → Determine correct layer (Domain vs Application), implement guard clause
/add-tests          → Unit test for valid and invalid amounts
/domain-event       → Add OrderAmountExceededDomainEvent if needed
/pre-pr             → Final verification
```

### "I need to make an architectural decision about caching"

```
/create-adr         → Document the decision, alternatives, and consequences
/code-review        → Review the ADR before acceptance
```
