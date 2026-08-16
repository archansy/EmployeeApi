using EmployeeService.Api.Authentication;
using EmployeeService.Api.Authorization;
using EmployeeService.Api.DependencyInjection;
using EmployeeService.Api.OpenApi;
using EmployeeService.Application.DependencyInjection;
using EmployeeService.Infrastructure.DependencyInjection;

namespace EmployeeService.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEmployeeService(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddApi(configuration)
            .AddApplication()
            .AddInfrastructure(configuration)
            .AddEmployeeOpenApi()
            .AddAuthenticationExtensionPoint(configuration)
            .AddAuthorizationExtensionPoint();

        return services;
    }
}
