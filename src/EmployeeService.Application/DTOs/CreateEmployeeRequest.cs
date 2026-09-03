namespace EmployeeService.Application.DTOs;

public sealed class CreateEmployeeRequest
{
    public string EmployeeNumber { get; init; } = string.Empty;

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Department { get; init; } = string.Empty;

    public string JobTitle { get; init; } = string.Empty;

    public DateOnly DateOfJoining { get; init; }
}
