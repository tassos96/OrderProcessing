using MediatR;
using Payments.Application.DTOs;

namespace Payments.Application.Commands.ProcessPayment;

public sealed record ProcessPaymentCommand(
    Guid OrderId,
    decimal Amount,
    string Currency) : IRequest<PaymentDto>;
