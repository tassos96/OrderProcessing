using BuildingBlocks.Application.Abstractions;

namespace BuildingBlocks.Infrastructure.Persistence;

public sealed class UnitOfWork<TContext>(TContext context) : IUnitOfWork
    where TContext : BaseDbContext
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => context.SaveChangesAsync(cancellationToken);
}
