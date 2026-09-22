using BuildingBlocks.Application.Envelope;
using Inventory.Application.Commands.ReserveInventory;
using Inventory.Application.DTOs;
using Inventory.Application.Queries.CheckAvailability;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OrderProcessing.Api.Controllers;

[ApiController]
[Route("api/inventory")]
public sealed class InventoryController(ISender sender) : ControllerBase
{
    [HttpPost("reserve")]
    [Authorize(Policy = "Inventory.Write")]
    [ProducesResponseType(typeof(ResponseEnvelope<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Reserve(
        [FromBody] RequestEnvelope<ReserveInventoryCommand> request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(request.Payload, cancellationToken);
        return Ok(ResponseEnvelope<bool>.Success(result));
    }

    [HttpGet("availability/{sku}")]
    [Authorize(Policy = "Inventory.Read")]
    [ProducesResponseType(typeof(ResponseEnvelope<InventoryItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckAvailability(string sku, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CheckAvailabilityQuery(sku), cancellationToken);
        if (result is null)
            return NotFound(ResponseEnvelope<InventoryItemDto>.Failure("SKU_NOT_FOUND", $"SKU '{sku}' was not found."));

        return Ok(ResponseEnvelope<InventoryItemDto>.Success(result));
    }
}
