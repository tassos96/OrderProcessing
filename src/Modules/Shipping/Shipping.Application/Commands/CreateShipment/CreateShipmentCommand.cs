using MediatR;
using Shipping.Application.DTOs;

namespace Shipping.Application.Commands.CreateShipment;

public sealed record CreateShipmentCommand(
    Guid OrderId,
    string Street,
    string City,
    string State,
    string PostalCode,
    string Country) : IRequest<ShipmentDto>;
