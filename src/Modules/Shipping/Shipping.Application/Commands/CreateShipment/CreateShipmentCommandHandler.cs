using MediatR;
using Shipping.Application.DTOs;

namespace Shipping.Application.Commands.CreateShipment;

public sealed class CreateShipmentCommandHandler : IRequestHandler<CreateShipmentCommand, ShipmentDto>
{
    public Task<ShipmentDto> Handle(CreateShipmentCommand request, CancellationToken cancellationToken)
    {
        // TODO: Create Shipment aggregate, call carrier service for label, persist
        throw new NotImplementedException();
    }
}
