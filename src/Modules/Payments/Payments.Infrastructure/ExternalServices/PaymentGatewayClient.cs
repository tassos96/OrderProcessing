using Payments.Application.Abstractions;

namespace Payments.Infrastructure.ExternalServices;

public sealed class PaymentGatewayClient(IPaymentGatewayApi gatewayApi) : IPaymentGateway
{
    public Task<GatewayChargeResult> ChargeAsync(decimal amount, string currency, CancellationToken cancellationToken = default)
    {
        // TODO: Call gatewayApi.ChargeAsync(), map response
        // TODO: Configure retry/timeout/circuit-breaker policies according to provider SLA
        throw new NotImplementedException();
    }

    public Task<GatewayRefundResult> RefundAsync(string transactionReference, CancellationToken cancellationToken = default)
    {
        // TODO: Call gatewayApi.RefundAsync(), map response
        throw new NotImplementedException();
    }
}
