using Refit;

namespace Payments.Infrastructure.ExternalServices;

public sealed record GatewayChargeRequest(decimal Amount, string Currency);
public sealed record GatewayChargeResponse(bool Success, string? TransactionReference, string? ErrorMessage);
public sealed record GatewayRefundRequest(string TransactionReference);
public sealed record GatewayRefundResponse(bool Success, string? ErrorMessage);

[Headers("Content-Type: application/json")]
public interface IPaymentGatewayApi
{
    [Post("/v1/charges")]
    Task<ApiResponse<GatewayChargeResponse>> ChargeAsync([Body] GatewayChargeRequest request, CancellationToken cancellationToken = default);

    [Post("/v1/refunds")]
    Task<ApiResponse<GatewayRefundResponse>> RefundAsync([Body] GatewayRefundRequest request, CancellationToken cancellationToken = default);
}
