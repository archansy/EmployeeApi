namespace EmployeeService.Application.DTOs;

public sealed class EmployeeDto
{
    public Guid Id { get; init; }

    public string EmployeeNumber { get; init; } = string.Empty;

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Department { get; init; } = string.Empty;

    public string JobTitle { get; init; } = string.Empty;

    public DateOnly DateOfJoining { get; init; }

    public bool IsActive { get; init; }

    public DateTime CreatedAtUtc { get; init; }

    public DateTime UpdatedAtUtc { get; init; }

    public string ConcurrencyToken { get; init; } = string.Empty;
}
