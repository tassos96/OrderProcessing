using MediatR;
using Orders.Application.DTOs;

namespace Orders.Application.Queries.GetOrderById;

public sealed record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderDto?>;
