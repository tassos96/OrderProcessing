using Orders.Application.Abstractions;

namespace Orders.Infrastructure.ExternalServices;

public sealed class PaymentServiceClient(IPaymentApi paymentApi) : IPaymentService
{
    public Task<PaymentResult> ProcessPaymentAsync(Guid orderId, decimal amount, string currency, CancellationToken cancellationToken = default)
    {
        // TODO: Call paymentApi.ProcessAsync(), map response, handle errors
        // TODO: Configure retry/timeout/circuit-breaker policies according to provider SLA
        throw new NotImplementedException();
    }
}
