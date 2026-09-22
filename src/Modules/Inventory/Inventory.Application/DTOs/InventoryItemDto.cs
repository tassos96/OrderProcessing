namespace Inventory.Application.DTOs;

public sealed record InventoryItemDto(
    Guid Id,
    string Sku,
    int AvailableQuantity,
    int ReservedQuantity);
