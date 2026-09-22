using Refit;

namespace Orders.Infrastructure.ExternalServices;

public sealed record ReserveStockRequest(Guid OrderId, string Sku, int Quantity);
public sealed record ReserveStockResponse(bool Success, string? Message);

[Headers("Content-Type: application/json")]
public interface IInventoryApi
{
    [Post("/api/inventory/reserve")]
    Task<ApiResponse<ReserveStockResponse>> ReserveAsync([Body] ReserveStockRequest request, CancellationToken cancellationToken = default);

    [Post("/api/inventory/release")]
    Task<ApiResponse<ReserveStockResponse>> ReleaseAsync([Body] ReserveStockRequest request, CancellationToken cancellationToken = default);
}
