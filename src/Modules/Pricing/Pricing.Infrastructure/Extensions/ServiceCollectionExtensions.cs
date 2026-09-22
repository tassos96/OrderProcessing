using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pricing.Application.Extensions;
using Pricing.Domain.Repositories;
using Pricing.Infrastructure.Persistence;
using Pricing.Infrastructure.Persistence.Repositories;

namespace Pricing.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPricingModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPricingApplication();

        services.AddDbContext<PricingDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("PricingDb"),
                sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", "pricing")));

        services.AddScoped<IPriceRuleRepository, PriceRuleRepository>();
        services.AddScoped<IUnitOfWork>(sp => new UnitOfWork<PricingDbContext>(sp.GetRequiredService<PricingDbContext>()));

        return services;
    }
}
