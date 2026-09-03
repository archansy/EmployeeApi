using EmployeeService.Domain.Entities;
using FluentAssertions;

namespace EmployeeService.UnitTests.Domain;

public sealed class EmployeeTests
{
    [Fact]
    public void Create_Should_Set_Expected_Values()
    {
        var now = DateTime.UtcNow;

        var employee = Employee.Create(
            "EMP-001",
            "John",
            "Doe",
            "john.doe@example.com",
            "Engineering",
            "Software Engineer",
            DateOnly.FromDateTime(now.Date),
            now);

        employee.EmployeeNumber.Should().Be("EMP-001");
        employee.Email.Should().Be("john.doe@example.com");
        employee.IsActive.Should().BeTrue();
        employee.CreatedAtUtc.Should().Be(now);
    }
}
