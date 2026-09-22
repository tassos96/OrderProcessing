using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Pricing.Domain.Entities;

namespace Pricing.Infrastructure.Persistence;

public sealed class PricingDbContext(DbContextOptions<PricingDbContext> options) : BaseDbContext(options)
{
    public DbSet<PriceRule> PriceRules => Set<PriceRule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("pricing");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PricingDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
