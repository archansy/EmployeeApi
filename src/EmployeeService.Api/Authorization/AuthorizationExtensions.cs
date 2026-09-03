namespace EmployeeService.Api.Authorization;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddAuthorizationExtensionPoint(this IServiceCollection services)
    {
        services.AddAuthorization();
        return services;
    }
}
