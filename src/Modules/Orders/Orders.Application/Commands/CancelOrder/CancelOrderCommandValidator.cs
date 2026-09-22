using FluentValidation;

namespace Orders.Application.Commands.CancelOrder;

public sealed class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
{
    public CancelOrderCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty().WithMessage("Order ID is required.");
        RuleFor(x => x.Reason).NotEmpty().WithMessage("Cancellation reason is required.");
    }
}
