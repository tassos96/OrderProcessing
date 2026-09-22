using Inventory.Application.DTOs;
using MediatR;

namespace Inventory.Application.Queries.CheckAvailability;

public sealed record CheckAvailabilityQuery(string Sku) : IRequest<InventoryItemDto?>;
