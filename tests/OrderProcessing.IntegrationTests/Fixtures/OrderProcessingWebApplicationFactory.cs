using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace OrderProcessing.IntegrationTests.Fixtures;

public class OrderProcessingWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            // TODO: Replace SQL Server DbContexts with InMemory for testing
            // TODO: Replace Refit clients with test doubles
            // TODO: Configure test authentication handler that bypasses JWT

            // Example:
            // services.RemoveAll<DbContextOptions<OrdersDbContext>>();
            // services.AddDbContext<OrdersDbContext>(opts => opts.UseInMemoryDatabase("TestDb"));
        });
    }
}
