namespace Orders.Application.DTOs;

public sealed record OrderDto(
    Guid Id,
    Guid CustomerId,
    string Status,
    IReadOnlyList<OrderItemDto> Items,
    decimal TotalAmount,
    AddressDto ShippingAddress);

public sealed record OrderItemDto(
    Guid Id,
    string Sku,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice);

public sealed record AddressDto(
    string Street,
    string City,
    string State,
    string PostalCode,
    string Country);

public sealed record CreateOrderItemDto(
    string Sku,
    string ProductName,
    int Quantity,
    decimal UnitPrice);
