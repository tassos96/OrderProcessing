using MediatR;
using Pricing.Application.DTOs;

namespace Pricing.Application.Queries.CalculatePrice;

public sealed record CalculatePriceQuery(string Sku, int Quantity) : IRequest<PriceCalculationDto>;
