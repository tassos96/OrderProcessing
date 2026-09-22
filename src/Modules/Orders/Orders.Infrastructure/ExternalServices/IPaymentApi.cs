using Refit;

namespace Orders.Infrastructure.ExternalServices;

public sealed record ProcessPaymentRequest(Guid OrderId, decimal Amount, string Currency);
public sealed record ProcessPaymentResponse(bool Success, string? TransactionReference, string? ErrorMessage);

[Headers("Content-Type: application/json")]
public interface IPaymentApi
{
    [Post("/api/payments/process")]
    Task<ApiResponse<ProcessPaymentResponse>> ProcessAsync([Body] ProcessPaymentRequest request, CancellationToken cancellationToken = default);
}
