using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orders.Application.Abstractions;
using Orders.Application.Extensions;
using Orders.Domain.Repositories;
using Orders.Infrastructure.ExternalServices;
using Orders.Infrastructure.Options;
using Orders.Infrastructure.Persistence;
using Orders.Infrastructure.Persistence.Repositories;
using Refit;

namespace Orders.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOrdersModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOrdersApplication();

        // Persistence
        services.AddDbContext<OrdersDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("OrdersDb"),
                sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", "orders")));

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IUnitOfWork>(sp => new UnitOfWork<OrdersDbContext>(sp.GetRequiredService<OrdersDbContext>()));

        // External services via Refit
        var inventoryOptions = new InventoryApiOptions();
        configuration.GetSection(InventoryApiOptions.SectionName).Bind(inventoryOptions);

        services.AddRefitClient<IInventoryApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(inventoryOptions.BaseUrl ?? "https://localhost"));

        services.AddRefitClient<IPaymentApi>()
            .ConfigureHttpClient(c =>
            {
                var opts = new PaymentApiOptions();
                configuration.GetSection(PaymentApiOptions.SectionName).Bind(opts);
                c.BaseAddress = new Uri(opts.BaseUrl ?? "https://localhost");
            });

        services.AddRefitClient<IShippingApi>()
            .ConfigureHttpClient(c =>
            {
                var opts = new ShippingApiOptions();
                configuration.GetSection(ShippingApiOptions.SectionName).Bind(opts);
                c.BaseAddress = new Uri(opts.BaseUrl ?? "https://localhost");
            });

        services.AddScoped<IInventoryService, InventoryServiceClient>();
        services.AddScoped<IPaymentService, PaymentServiceClient>();
        services.AddScoped<IShippingService, ShippingServiceClient>();

        return services;
    }
}
