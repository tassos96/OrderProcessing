using BuildingBlocks.Application.Envelope;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pricing.Application.DTOs;
using Pricing.Application.Queries.CalculatePrice;

namespace OrderProcessing.Api.Controllers;

[ApiController]
[Route("api/pricing")]
public sealed class PricingController(ISender sender) : ControllerBase
{
    [HttpGet("calculate")]
    [Authorize(Policy = "Pricing.Read")]
    [ProducesResponseType(typeof(ResponseEnvelope<PriceCalculationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Calculate(
        [FromQuery] string sku,
        [FromQuery] int quantity,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CalculatePriceQuery(sku, quantity), cancellationToken);
        return Ok(ResponseEnvelope<PriceCalculationDto>.Success(result));
    }
}
