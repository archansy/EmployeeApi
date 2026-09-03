using Microsoft.OpenApi.Models;

namespace EmployeeService.Api.OpenApi;

public static class OpenApiConfiguration
{
    public static IServiceCollection AddEmployeeOpenApi(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "EmployeeService API",
                Version = "v1",
                Description = "Employee management microservice API"
            });
        });

        return services;
    }
}
