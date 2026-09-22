namespace Pricing.Application.DTOs;

public sealed record PriceCalculationDto(
    string Sku,
    decimal BasePrice,
    decimal Discount,
    decimal Tax,
    decimal FinalPrice);
