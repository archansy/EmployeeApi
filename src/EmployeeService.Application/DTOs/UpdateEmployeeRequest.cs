namespace EmployeeService.Application.DTOs;

public sealed class UpdateEmployeeRequest
{
    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Department { get; init; } = string.Empty;

    public string JobTitle { get; init; } = string.Empty;

    public DateOnly DateOfJoining { get; init; }

    public bool IsActive { get; init; }

    public string ConcurrencyToken { get; init; } = string.Empty;
}
