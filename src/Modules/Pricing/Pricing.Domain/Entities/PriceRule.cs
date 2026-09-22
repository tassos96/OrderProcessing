using BuildingBlocks.Domain.Abstractions;
using BuildingBlocks.Domain.ValueObjects;
using Pricing.Domain.ValueObjects;

namespace Pricing.Domain.Entities;

public sealed class PriceRule : AggregateRoot<Guid>
{
    public string Sku { get; private set; } = default!;
    public Money BasePrice { get; private set; } = default!;
    public DiscountType DiscountType { get; private set; }
    public decimal DiscountValue { get; private set; }

    private PriceRule() { }

    public static PriceRule Create(string sku, Money basePrice, DiscountType discountType = DiscountType.None, decimal discountValue = 0)
    {
        return new PriceRule
        {
            Id = Guid.NewGuid(),
            Sku = sku,
            BasePrice = basePrice,
            DiscountType = discountType,
            DiscountValue = discountValue,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public Money CalculatePrice(int quantity)
    {
        // TODO: Implement pricing logic based on DiscountType
        // Percentage: BasePrice * (1 - DiscountValue/100) * quantity
        // FixedAmount: (BasePrice - DiscountValue) * quantity
        // BulkDiscount: Apply tiered pricing based on quantity
        throw new NotImplementedException();
    }
}
