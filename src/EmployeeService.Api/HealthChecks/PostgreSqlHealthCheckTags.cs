namespace EmployeeService.Api.HealthChecks;

public static class PostgreSqlHealthCheckTags
{
    public const string Readiness = "readiness";

    public const string Liveness = "liveness";
}
