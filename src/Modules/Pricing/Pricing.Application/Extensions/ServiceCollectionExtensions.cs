using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Pricing.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPricingApplication(this IServiceCollection services)
    {
        var assembly = typeof(ServiceCollectionExtensions).Assembly;
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);
        return services;
    }
}
