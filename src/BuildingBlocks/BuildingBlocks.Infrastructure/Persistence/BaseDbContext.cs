using BuildingBlocks.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Infrastructure.Persistence;

public abstract class BaseDbContext(DbContextOptions options) : DbContext(options)
{
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // TODO: Before saving, collect domain events from all tracked aggregates
        // TODO: After saving, dispatch domain events via MediatR
        // TODO: Consider publishing to outbox for reliable cross-module messaging

        UpdateAuditableEntities();

        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditableEntities()
    {
        var entries = ChangeTracker.Entries<Entity<Guid>>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
                entry.Entity.GetType().GetProperty("CreatedAtUtc")?.SetValue(entry.Entity, DateTime.UtcNow);

            if (entry.State == EntityState.Modified)
                entry.Entity.GetType().GetProperty("UpdatedAtUtc")?.SetValue(entry.Entity, DateTime.UtcNow);
        }
    }
}
