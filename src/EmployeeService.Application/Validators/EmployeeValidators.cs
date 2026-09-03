using EmployeeService.Application.Features.Employees.Commands.CreateEmployee;
using EmployeeService.Application.Features.Employees.Commands.UpdateEmployee;
using EmployeeService.Application.Features.Employees.Queries.GetEmployees;
using FluentValidation;

namespace EmployeeService.Application.Validators;

public sealed class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeCommandValidator()
    {
        RuleFor(x => x.EmployeeNumber).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Department).NotEmpty();
        RuleFor(x => x.JobTitle).NotEmpty();
        RuleFor(x => x.DateOfJoining)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow));
    }
}

public sealed class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Department).NotEmpty();
        RuleFor(x => x.JobTitle).NotEmpty();
        RuleFor(x => x.ConcurrencyToken).NotEmpty();
        RuleFor(x => x.DateOfJoining)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow));
    }
}

public sealed class GetEmployeesQueryValidator : AbstractValidator<GetEmployeesQuery>
{
    public GetEmployeesQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 200);
    }
}
