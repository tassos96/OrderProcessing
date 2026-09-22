using BuildingBlocks.Application.Envelope;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payments.Application.Commands.ProcessPayment;
using Payments.Application.DTOs;

namespace OrderProcessing.Api.Controllers;

[ApiController]
[Route("api/payments")]
public sealed class PaymentsController(ISender sender) : ControllerBase
{
    [HttpPost("process")]
    [Authorize(Policy = "Payments.Process")]
    [ProducesResponseType(typeof(ResponseEnvelope<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Process(
        [FromBody] RequestEnvelope<ProcessPaymentCommand> request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(request.Payload, cancellationToken);
        return Ok(ResponseEnvelope<PaymentDto>.Success(result));
    }
}
