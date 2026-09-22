using FluentValidation;

namespace Pricing.Application.Queries.CalculatePrice;

public sealed class CalculatePriceQueryValidator : AbstractValidator<CalculatePriceQuery>
{
    public CalculatePriceQueryValidator()
    {
        RuleFor(x => x.Sku).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
