using BuildingBlocks.Application.Envelope;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipping.Application.Commands.CreateShipment;
using Shipping.Application.DTOs;

namespace OrderProcessing.Api.Controllers;

[ApiController]
[Route("api/shipping")]
public sealed class ShippingController(ISender sender) : ControllerBase
{
    [HttpPost("create")]
    [Authorize(Policy = "Shipping.Create")]
    [ProducesResponseType(typeof(ResponseEnvelope<ShipmentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create(
        [FromBody] RequestEnvelope<CreateShipmentCommand> request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(request.Payload, cancellationToken);
        return Ok(ResponseEnvelope<ShipmentDto>.Success(result));
    }
}
