using BuildingBlocks.Application.Extensions;
using BuildingBlocks.Infrastructure.Extensions;
using Inventory.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OrderProcessing.Api.Middleware;
using OrderProcessing.Api.Options;
using Orders.Infrastructure.Extensions;
using Payments.Infrastructure.Extensions;
using Pricing.Infrastructure.Extensions;
using Scalar.AspNetCore;
using Serilog;
using Shipping.Infrastructure.Extensions;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Serilog
    builder.Host.UseSerilog((context, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration));

    // Authentication
    var jwtOptions = new JwtOptions();
    builder.Configuration.GetSection(JwtOptions.SectionName).Bind(jwtOptions);

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            if (!string.IsNullOrEmpty(jwtOptions.SigningKey))
            {
                var key = new SymmetricSecurityKey(Convert.FromBase64String(jwtOptions.SigningKey));
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Authority,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            }
            else
            {
                options.Authority = jwtOptions.Authority;
                options.Audience = jwtOptions.Audience;
                options.RequireHttpsMetadata = jwtOptions.RequireHttpsMetadata;
            }
        });

    // Authorization
    builder.Services.AddAuthorizationBuilder()
        .AddPolicy("Orders.Read", policy => policy.RequireClaim("permission", "orders.read"))
        .AddPolicy("Orders.Write", policy => policy.RequireClaim("permission", "orders.write"))
        .AddPolicy("Orders.Cancel", policy => policy.RequireClaim("permission", "orders.cancel"))
        .AddPolicy("Inventory.Read", policy => policy.RequireClaim("permission", "inventory.read"))
        .AddPolicy("Inventory.Write", policy => policy.RequireClaim("permission", "inventory.write"))
        .AddPolicy("Pricing.Read", policy => policy.RequireClaim("permission", "pricing.read"))
        .AddPolicy("Payments.Process", policy => policy.RequireClaim("permission", "payments.process"))
        .AddPolicy("Shipping.Create", policy => policy.RequireClaim("permission", "shipping.create"));

    // OpenAPI + Swagger + Scalar
    builder.Services.AddOpenApi();

    // Controllers
    builder.Services.AddControllers();

    // Exception handling
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    // BuildingBlocks
    builder.Services.AddBuildingBlocksApplication();
    builder.Services.AddBuildingBlocksInfrastructure();

    // Modules
    builder.Services.AddOrdersModule(builder.Configuration);
    builder.Services.AddInventoryModule(builder.Configuration);
    builder.Services.AddPricingModule(builder.Configuration);
    builder.Services.AddPaymentsModule(builder.Configuration);
    builder.Services.AddShippingModule(builder.Configuration);

    // Health checks
    builder.Services.AddHealthChecks();
    // TODO: Add DbContext health checks per module
    // TODO: Add external service health checks

    var app = builder.Build();

    // Middleware pipeline
    app.UseExceptionHandler();
    app.UseMiddleware<CorrelationIdMiddleware>();
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "Order Processing API v1");
        });
        app.MapScalarApiReference();
    }

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    // Health check endpoints
    app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        Predicate = _ => false // Liveness: always healthy if process is running
    });
    app.MapHealthChecks("/health/ready");

    Log.Information("Starting Order Processing API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// Required for WebApplicationFactory in integration tests
public partial class Program;
