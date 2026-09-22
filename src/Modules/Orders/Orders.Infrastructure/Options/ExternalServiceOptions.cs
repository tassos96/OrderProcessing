namespace Orders.Infrastructure.Options;

public sealed class InventoryApiOptions
{
    public const string SectionName = "ExternalServices:Inventory";
    public string BaseUrl { get; init; } = default!;
    public int TimeoutSeconds { get; init; } = 30;
}

public sealed class PaymentApiOptions
{
    public const string SectionName = "ExternalServices:Payment";
    public string BaseUrl { get; init; } = default!;
    public int TimeoutSeconds { get; init; } = 30;
}

public sealed class ShippingApiOptions
{
    public const string SectionName = "ExternalServices:Shipping";
    public string BaseUrl { get; init; } = default!;
    public int TimeoutSeconds { get; init; } = 30;
}
