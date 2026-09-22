using BuildingBlocks.Application.Envelope;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orders.Application.Commands.CancelOrder;
using Orders.Application.Commands.CreateOrder;
using Orders.Application.DTOs;
using Orders.Application.Queries.GetOrderById;

namespace OrderProcessing.Api.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController(ISender sender) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = "Orders.Write")]
    [ProducesResponseType(typeof(ResponseEnvelope<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseEnvelope<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] RequestEnvelope<CreateOrderCommand> request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(request.Payload, cancellationToken);
        return Ok(ResponseEnvelope<OrderDto>.Success(result));
    }

    [HttpGet("{orderId:guid}")]
    [Authorize(Policy = "Orders.Read")]
    [ProducesResponseType(typeof(ResponseEnvelope<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseEnvelope<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid orderId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetOrderByIdQuery(orderId), cancellationToken);
        if (result is null)
            return NotFound(ResponseEnvelope<OrderDto>.Failure("ORDER_NOT_FOUND", $"Order '{orderId}' was not found."));

        return Ok(ResponseEnvelope<OrderDto>.Success(result));
    }

    [HttpPost("{orderId:guid}/cancel")]
    [Authorize(Policy = "Orders.Cancel")]
    [ProducesResponseType(typeof(ResponseEnvelope<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseEnvelope<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Cancel(
        Guid orderId,
        [FromBody] RequestEnvelope<CancelOrderRequest> request,
        CancellationToken cancellationToken)
    {
        await sender.Send(new CancelOrderCommand(orderId, request.Payload.Reason), cancellationToken);
        return Ok(ResponseEnvelope<object>.Success(new { orderId, status = "Cancelled" }));
    }
}

public sealed record CancelOrderRequest(string Reason);
