namespace Payments.Infrastructure.Options;

public sealed class PaymentGatewayApiOptions
{
    public const string SectionName = "ExternalServices:PaymentGateway";
    public string BaseUrl { get; init; } = default!;
    public int TimeoutSeconds { get; init; } = 30;
}
