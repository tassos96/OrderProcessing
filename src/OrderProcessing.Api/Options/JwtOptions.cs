namespace OrderProcessing.Api.Options;

public sealed class JwtOptions
{
    public const string SectionName = "Authentication:Jwt";
    public string Authority { get; init; } = default!;
    public string Audience { get; init; } = default!;
    public bool RequireHttpsMetadata { get; init; } = true;
    public string? SigningKey { get; init; }
}
