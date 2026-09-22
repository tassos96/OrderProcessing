using Payments.Domain.Entities;
using Payments.Domain.Repositories;

namespace Payments.Infrastructure.Persistence.Repositories;

public sealed class PaymentRepository(PaymentsDbContext context) : IPaymentRepository
{
    public Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // TODO: Implement
        throw new NotImplementedException();
    }

    public Task<Payment?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        // TODO: Implement
        throw new NotImplementedException();
    }

    public async Task AddAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        await context.Payments.AddAsync(payment, cancellationToken);
    }

    public void Update(Payment payment)
    {
        context.Payments.Update(payment);
    }
}
