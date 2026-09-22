using Pricing.Domain.Entities;
using Pricing.Domain.Repositories;

namespace Pricing.Infrastructure.Persistence.Repositories;

public sealed class PriceRuleRepository(PricingDbContext context) : IPriceRuleRepository
{
    public Task<PriceRule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // TODO: Implement
        throw new NotImplementedException();
    }

    public Task<PriceRule?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        // TODO: Implement with EF Core query
        throw new NotImplementedException();
    }

    public async Task AddAsync(PriceRule rule, CancellationToken cancellationToken = default)
    {
        await context.PriceRules.AddAsync(rule, cancellationToken);
    }
}
