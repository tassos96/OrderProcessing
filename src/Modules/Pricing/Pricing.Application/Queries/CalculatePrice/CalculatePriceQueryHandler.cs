using MediatR;
using Pricing.Application.DTOs;

namespace Pricing.Application.Queries.CalculatePrice;

public sealed class CalculatePriceQueryHandler : IRequestHandler<CalculatePriceQuery, PriceCalculationDto>
{
    public Task<PriceCalculationDto> Handle(CalculatePriceQuery request, CancellationToken cancellationToken)
    {
        // TODO: Load PriceRule by SKU, call CalculatePrice(), apply taxes, return DTO
        throw new NotImplementedException();
    }
}
