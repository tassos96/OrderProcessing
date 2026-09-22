namespace Orders.Application.Abstractions;

public interface IPaymentService
{
    Task<PaymentResult> ProcessPaymentAsync(Guid orderId, decimal amount, string currency, CancellationToken cancellationToken = default);
}

public sealed record PaymentResult(bool Success, string? TransactionReference, string? ErrorMessage);
