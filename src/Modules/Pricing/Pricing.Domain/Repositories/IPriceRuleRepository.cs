using Pricing.Domain.Entities;

namespace Pricing.Domain.Repositories;

public interface IPriceRuleRepository
{
    Task<PriceRule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PriceRule?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task AddAsync(PriceRule rule, CancellationToken cancellationToken = default);
}
