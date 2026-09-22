using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Shipping.Application.Abstractions;
using Shipping.Application.Extensions;
using Shipping.Domain.Repositories;
using Shipping.Infrastructure.ExternalServices;
using Shipping.Infrastructure.Options;
using Shipping.Infrastructure.Persistence;
using Shipping.Infrastructure.Persistence.Repositories;

namespace Shipping.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddShippingModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddShippingApplication();

        services.AddDbContext<ShippingDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("ShippingDb"),
                sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", "shipping")));

        services.AddScoped<IShipmentRepository, ShipmentRepository>();
        services.AddScoped<IUnitOfWork>(sp => new UnitOfWork<ShippingDbContext>(sp.GetRequiredService<ShippingDbContext>()));

        var carrierOptions = new ShippingCarrierApiOptions();
        configuration.GetSection(ShippingCarrierApiOptions.SectionName).Bind(carrierOptions);

        services.AddRefitClient<IShippingCarrierApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(carrierOptions.BaseUrl ?? "https://localhost"));

        services.AddScoped<IShippingCarrierService, ShippingCarrierClient>();

        return services;
    }
}
