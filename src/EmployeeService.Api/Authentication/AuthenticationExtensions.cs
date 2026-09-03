using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeService.Api.Authentication;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddAuthenticationExtensionPoint(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
        {
            var issuer = configuration["Jwt:Issuer"] ?? "EmployeeService";
            var audience = configuration["Jwt:Audience"] ?? "EmployeeHubFrontend";
            var signingKey = Environment.GetEnvironmentVariable("EMPLOYEE_SERVICE__JWT_SIGNING_KEY")
                ?? configuration["Jwt:SigningKey"]
                ?? throw new InvalidOperationException("JWT signing key is missing.");

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30),
                NameClaimType = JwtRegisteredClaimNames.UniqueName,
                RoleClaimType = ClaimTypes.Role
            };
        });

        return services;
    }
}
