namespace EmployeeService.Api.Common;

public sealed class ApiErrorResponse
{
    public required string Code { get; init; }

    public required string Message { get; init; }

    public object? Details { get; init; }
}
