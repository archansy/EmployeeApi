using EmployeeService.Application.Features.Employees.Commands.CreateEmployee;
using EmployeeService.Application.Validators;
using FluentAssertions;

namespace EmployeeService.UnitTests.Application;

public sealed class CreateEmployeeValidatorTests
{
    [Fact]
    public void Validate_Should_Fail_For_Future_Date()
    {
        var validator = new CreateEmployeeCommandValidator();
        var command = new CreateEmployeeCommand(
            "EMP-001",
            "John",
            "Doe",
            "john.doe@example.com",
            "Engineering",
            "Developer",
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)));

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateEmployeeCommand.DateOfJoining));
    }
}
