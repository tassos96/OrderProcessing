using FluentValidation;

namespace Orders.Application.Commands.CreateOrder;

public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required.");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Order must contain at least one item.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.Sku)
                .NotEmpty().WithMessage("Product SKU is required.");

            item.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");

            item.RuleFor(i => i.UnitPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Unit price cannot be negative.");
        });

        RuleFor(x => x.ShippingAddress).NotNull().WithMessage("Shipping address is required.");
        RuleFor(x => x.ShippingAddress.Street).NotEmpty().When(x => x.ShippingAddress is not null);
        RuleFor(x => x.ShippingAddress.City).NotEmpty().When(x => x.ShippingAddress is not null);
        RuleFor(x => x.ShippingAddress.Country).NotEmpty().When(x => x.ShippingAddress is not null);
    }
}
