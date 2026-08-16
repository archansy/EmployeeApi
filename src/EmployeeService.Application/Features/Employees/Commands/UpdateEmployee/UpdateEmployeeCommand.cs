using EmployeeService.Application.DTOs;
using MediatR;

namespace EmployeeService.Application.Features.Employees.Commands.UpdateEmployee;

public sealed record UpdateEmployeeCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Department,
    string JobTitle,
    DateOnly DateOfJoining,
    bool IsActive,
    string ConcurrencyToken) : IRequest<EmployeeDto>;
