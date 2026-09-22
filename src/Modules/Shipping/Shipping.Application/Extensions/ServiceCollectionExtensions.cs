using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Shipping.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddShippingApplication(this IServiceCollection services)
    {
        var assembly = typeof(ServiceCollectionExtensions).Assembly;
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);
        return services;
    }
}
