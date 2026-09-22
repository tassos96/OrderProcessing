using MediatR;
using Payments.Application.DTOs;

namespace Payments.Application.Commands.ProcessPayment;

public sealed class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, PaymentDto>
{
    public Task<PaymentDto> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        // TODO: Create Payment aggregate, call gateway, update status, persist
        throw new NotImplementedException();
    }
}
