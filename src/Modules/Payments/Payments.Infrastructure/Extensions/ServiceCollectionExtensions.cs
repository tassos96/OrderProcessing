using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payments.Application.Abstractions;
using Payments.Application.Extensions;
using Payments.Domain.Repositories;
using Payments.Infrastructure.ExternalServices;
using Payments.Infrastructure.Options;
using Payments.Infrastructure.Persistence;
using Payments.Infrastructure.Persistence.Repositories;
using Refit;

namespace Payments.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPaymentsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPaymentsApplication();

        services.AddDbContext<PaymentsDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("PaymentsDb"),
                sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", "payments")));

        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IUnitOfWork>(sp => new UnitOfWork<PaymentsDbContext>(sp.GetRequiredService<PaymentsDbContext>()));

        var gatewayOptions = new PaymentGatewayApiOptions();
        configuration.GetSection(PaymentGatewayApiOptions.SectionName).Bind(gatewayOptions);

        services.AddRefitClient<IPaymentGatewayApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(gatewayOptions.BaseUrl ?? "https://localhost"));

        services.AddScoped<IPaymentGateway, PaymentGatewayClient>();

        return services;
    }
}
