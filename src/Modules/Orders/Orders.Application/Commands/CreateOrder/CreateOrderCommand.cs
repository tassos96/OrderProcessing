using MediatR;
using Orders.Application.DTOs;

namespace Orders.Application.Commands.CreateOrder;

public sealed record CreateOrderCommand(
    Guid CustomerId,
    List<CreateOrderItemDto> Items,
    AddressDto ShippingAddress) : IRequest<OrderDto>;
