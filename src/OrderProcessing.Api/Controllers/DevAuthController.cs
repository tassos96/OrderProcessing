using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BuildingBlocks.Application.Envelope;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OrderProcessing.Api.Options;

namespace OrderProcessing.Api.Controllers;

[ApiController]
[Route("api/dev")]
public sealed class DevAuthController(IConfiguration configuration, IHostEnvironment environment) : ControllerBase
{
    private static readonly Dictionary<string, TestUser> TestUsers = new()
    {
        ["admin"] = new("admin", "Platform Administrator", [
            "orders.read", "orders.write", "orders.cancel",
            "inventory.read", "inventory.write",
            "pricing.read",
            "payments.process",
            "shipping.create"
        ]),
        ["order-manager"] = new("order-manager", "Order Lifecycle Manager", [
            "orders.read", "orders.write", "orders.cancel"
        ]),
        ["warehouse-worker"] = new("warehouse-worker", "Warehouse Operations", [
            "inventory.read", "inventory.write"
        ]),
        ["finance-analyst"] = new("finance-analyst", "Financial Operations", [
            "orders.read", "payments.process", "pricing.read"
        ]),
        ["shipping-clerk"] = new("shipping-clerk", "Shipping Operations", [
            "shipping.create", "orders.read"
        ]),
        ["readonly-user"] = new("readonly-user", "Read-Only Dashboard Access", [
            "orders.read", "inventory.read", "pricing.read"
        ])
    };

    [HttpGet("users")]
    [ProducesResponseType(typeof(ResponseEnvelope<IEnumerable<TestUser>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseEnvelope<object>), StatusCodes.Status404NotFound)]
    public IActionResult GetUsers()
    {
        if (!environment.IsDevelopment())
            return NotFound();

        return Ok(ResponseEnvelope<IEnumerable<TestUser>>.Success(TestUsers.Values));
    }

    [HttpPost("token")]
    [ProducesResponseType(typeof(ResponseEnvelope<TokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseEnvelope<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseEnvelope<object>), StatusCodes.Status404NotFound)]
    public IActionResult CreateToken([FromBody] TokenRequest request)
    {
        if (!environment.IsDevelopment())
            return NotFound();

        var jwtOptions = new JwtOptions();
        configuration.GetSection(JwtOptions.SectionName).Bind(jwtOptions);

        if (string.IsNullOrEmpty(jwtOptions.SigningKey))
            return BadRequest(ResponseEnvelope<object>.Failure(
                "DEV_AUTH_DISABLED",
                "Development signing key is not configured. Set Authentication:Jwt:SigningKey in appsettings.Development.json."));

        if (!TestUsers.TryGetValue(request.Username, out var user))
            return BadRequest(ResponseEnvelope<object>.Failure(
                "UNKNOWN_USER",
                $"User '{request.Username}' not found. Use GET /api/dev/users to see available test users."));

        var key = new SymmetricSecurityKey(Convert.FromBase64String(jwtOptions.SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Username),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("name", user.Description)
        };

        foreach (var permission in user.Permissions)
            claims.Add(new Claim("permission", permission));

        var token = new JwtSecurityToken(
            issuer: jwtOptions.Authority,
            audience: jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(request.ExpiresInHours ?? 1),
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(ResponseEnvelope<TokenResponse>.Success(
            new TokenResponse(tokenString, token.ValidTo, user.Username, user.Permissions)));
    }

    public sealed record TestUser(string Username, string Description, string[] Permissions);
    public sealed record TokenRequest(string Username, int? ExpiresInHours = 1);
    public sealed record TokenResponse(string Token, DateTime ExpiresAtUtc, string Username, string[] Permissions);
}
