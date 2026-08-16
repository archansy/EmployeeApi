using EmployeeService.Application.DTOs;
using MediatR;

namespace EmployeeService.Application.Features.Employees.Commands.CreateEmployee;

public sealed record CreateEmployeeCommand(
    string EmployeeNumber,
    string FirstName,
    string LastName,
    string Email,
    string Department,
    string JobTitle,
    DateOnly DateOfJoining) : IRequest<EmployeeDto>;
