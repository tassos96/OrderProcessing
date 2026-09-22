namespace Payments.Application.Abstractions;

public interface IPaymentGateway
{
    Task<GatewayChargeResult> ChargeAsync(decimal amount, string currency, CancellationToken cancellationToken = default);
    Task<GatewayRefundResult> RefundAsync(string transactionReference, CancellationToken cancellationToken = default);
}

public sealed record GatewayChargeResult(bool Success, string? TransactionReference, string? ErrorMessage);
public sealed record GatewayRefundResult(bool Success, string? ErrorMessage);
