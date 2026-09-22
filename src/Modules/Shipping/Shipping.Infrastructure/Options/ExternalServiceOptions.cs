namespace Shipping.Infrastructure.Options;

public sealed class ShippingCarrierApiOptions
{
    public const string SectionName = "ExternalServices:ShippingCarrier";
    public string BaseUrl { get; init; } = default!;
    public int TimeoutSeconds { get; init; } = 30;
}
