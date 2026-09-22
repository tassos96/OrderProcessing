# Database Change

Add or modify a domain entity with its EF Core configuration, DbContext registration, and repository.

## When to use

- Adding a new table/entity to a module
- Adding properties to an existing entity
- Changing EF Core configuration (indexes, constraints, column types)
- Adding a new repository

## Before you start

1. Determine which module owns this entity.
2. Read the module's DbContext: `src/Modules/{Module}/{Module}.Infrastructure/Persistence/{Module}DbContext.cs`
3. Read an existing EF configuration as the template:
   - `src/Modules/Orders/Orders.Infrastructure/Persistence/Configurations/OrderConfiguration.cs`
   - `src/Modules/Orders/Orders.Infrastructure/Persistence/Configurations/OrderItemConfiguration.cs`
4. Read an existing entity: `src/Modules/Orders/Orders.Domain/Entities/Order.cs`
5. Read an existing repository:
   - Interface: `src/Modules/Orders/Orders.Domain/Repositories/IOrderRepository.cs`
   - Implementation: `src/Modules/Orders/Orders.Infrastructure/Persistence/Repositories/OrderRepository.cs`
6. Read `src/BuildingBlocks/BuildingBlocks.Domain/Abstractions/Entity.cs` and `AggregateRoot.cs` for base types.

## Steps

1. **Create or modify the domain entity** in `src/Modules/{Module}/{Module}.Domain/Entities/`:
   - Inherit from `AggregateRoot<Guid>` (for aggregates) or `Entity<Guid>` (for child entities)
   - Add a private parameterless constructor: `private {Entity}() { }`
   - Add a static factory method: `public static {Entity} Create(...)` that sets `Id = Guid.NewGuid()` and `CreatedAtUtc = DateTime.UtcNow`
   - Use value objects for domain concepts (following patterns in `Orders.Domain.ValueObjects/`)
   - Properties should be `{ get; private set; }` with `= default!` for required reference types

2. **Create the EF Core configuration** in `src/Modules/{Module}/{Module}.Infrastructure/Persistence/Configurations/{Entity}Configuration.cs`:
   - Implement `IEntityTypeConfiguration<{Entity}>` as a `sealed class`
   - Set table name and key: `builder.ToTable("{Entities}").HasKey(e => e.Id);`
   - Map value objects with `builder.OwnsOne(e => e.{ValueObject}, vo => { ... })` with column name overrides
   - Map enums with `.HasConversion<string>().HasMaxLength(50)`
   - Map `Money` as owned type with `Amount` (precision 18,2) and `Currency` (maxlength 3)
   - Set `HasMaxLength` on all string properties
   - Ignore `DomainEvents`: `builder.Ignore(e => e.DomainEvents)`
   - Add indexes where appropriate

3. **Add DbSet to the module's DbContext:**
   ```
   public DbSet<{Entity}> {Entities} => Set<{Entity}>();
   ```
   The `ApplyConfigurationsFromAssembly` call already picks up configurations from the assembly — no additional registration needed.

4. **Create repository interface** (if needed) in `src/Modules/{Module}/{Module}.Domain/Repositories/I{Entity}Repository.cs`:
   - `Task<{Entity}?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);`
   - `Task AddAsync({Entity} entity, CancellationToken cancellationToken = default);`
   - `void Update({Entity} entity);`
   - Add query methods as needed

5. **Create repository implementation** in `src/Modules/{Module}/{Module}.Infrastructure/Persistence/Repositories/{Entity}Repository.cs`:
   - `sealed class` with primary constructor injecting `{Module}DbContext`
   - `AddAsync`: `await context.{Entities}.AddAsync(entity, cancellationToken);`
   - `Update`: `context.{Entities}.Update(entity);`
   - Query methods: use LINQ with `Include()` for related entities

6. **Register in DI** in `src/Modules/{Module}/{Module}.Infrastructure/Extensions/ServiceCollectionExtensions.cs`:
   - `services.AddScoped<I{Entity}Repository, {Entity}Repository>();`

7. **Migration note:** This repository does not have EF Core migrations set up yet. Add a TODO comment:
   ```
   // TODO: Create migration: dotnet ef migrations add Add{Entity} --project src/Modules/{Module}/{Module}.Infrastructure --startup-project src/OrderProcessing.Api --context {Module}DbContext
   ```

## Architecture rules

- Entity definitions live in **Domain** — no EF Core attributes or annotations
- EF Core configurations live in **Infrastructure** — all mapping is code-based via `IEntityTypeConfiguration<T>`
- Repository interfaces live in **Domain** — they define the contract
- Repository implementations live in **Infrastructure** — they use the DbContext
- The DbContext uses the module's schema: `modelBuilder.HasDefaultSchema("{module_lowercase}")`

## Checklist

- [ ] Entity has private constructor and factory method
- [ ] Entity inherits `AggregateRoot<Guid>` or `Entity<Guid>`
- [ ] EF configuration is a separate `IEntityTypeConfiguration<T>` class (not in `OnModelCreating`)
- [ ] All string properties have `HasMaxLength`
- [ ] `DomainEvents` ignored in configuration
- [ ] Value objects mapped with `OwnsOne`
- [ ] Enums mapped with `HasConversion<string>()`
- [ ] Repository registered in DI
- [ ] No EF Core dependencies in Domain or Application layer
- [ ] `dotnet build` succeeds

## Things NOT to do

- Do not put EF Core attributes (`[Table]`, `[Column]`, `[Required]`) on domain entities
- Do not create a generic repository pattern — use specific repositories per aggregate
- Do not add migrations if the migration infrastructure isn't set up (add a TODO instead)
- Do not reference `Microsoft.EntityFrameworkCore` from Domain or Application projects

## Related skills

- `/new-module` — includes DbContext and initial entity creation
- `/new-endpoint` — endpoints may need new repository methods
- `/architecture-check` — verify no EF Core leaking into Domain
- `/add-tests` — test the entity's domain logic
