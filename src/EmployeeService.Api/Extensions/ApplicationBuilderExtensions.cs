using EmployeeService.Api.HealthChecks;
using EmployeeService.Api.Middleware;
using Serilog;

namespace EmployeeService.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseEmployeeService(this WebApplication app)
    {
        app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
        app.UseMiddleware<SensitiveRequestLoggingMiddleware>();

        app.UseSerilogRequestLogging();
        app.UseHttpsRedirection();
        app.UseCors("Frontend");
        app.UseRateLimiter();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = _ => true
        });

        app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = registration => registration.Tags.Contains(PostgreSqlHealthCheckTags.Readiness)
        });

        return app;
    }
}
