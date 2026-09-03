using EmployeeService.Api.Common;
using EmployeeService.Api.Filters;
using EmployeeService.Api.HealthChecks;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace EmployeeService.Api.DependencyInjection;

public static class ApiServiceRegistration
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = Environment.GetEnvironmentVariable("EMPLOYEE_SERVICE__CONNECTION_STRING")
            ?? configuration.GetConnectionString("EmployeeService")
            ?? throw new InvalidOperationException("EmployeeService connection string is not configured.");

        services.AddControllers(options => options.Filters.Add<ValidationProblemDetailsFilter>());
        services.AddProblemDetails();

        var allowedOrigin = configuration["Cors:AllowedOrigin"] ?? "http://localhost:3000";
        services.AddCors(options =>
        {
            options.AddPolicy("Frontend", policy =>
            {
                policy.WithOrigins(allowedOrigin)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.ContentType = "application/json";
                await context.HttpContext.Response.WriteAsJsonAsync(new ApiErrorResponse
                {
                    Code = "rate_limited",
                    Message = "Too many requests. Please try again later.",
                    Details = null
                }, token);
            };

            options.AddPolicy("auth-login", httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: $"login:{httpContext.Connection.RemoteIpAddress}",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 10,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));
        });

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var details = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value!.Errors.Select(e => e.ErrorMessage).ToArray());

                return new BadRequestObjectResult(new ApiErrorResponse
                {
                    Code = "validation_error",
                    Message = "One or more validation errors occurred.",
                    Details = details
                });
            };
        });

        services.AddHealthChecks()
            .AddNpgSql(connectionString, tags: [PostgreSqlHealthCheckTags.Readiness]);

        services.AddScoped<ValidationProblemDetailsFilter>();
        services.AddTransient<Middleware.GlobalExceptionHandlingMiddleware>();
        services.AddTransient<Middleware.SensitiveRequestLoggingMiddleware>();

        return services;
    }
}
