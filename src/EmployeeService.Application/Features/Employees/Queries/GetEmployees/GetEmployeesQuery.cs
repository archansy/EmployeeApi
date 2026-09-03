using EmployeeService.Application.DTOs;
using MediatR;

namespace EmployeeService.Application.Features.Employees.Queries.GetEmployees;

public sealed record GetEmployeesQuery(int PageNumber = 1, int PageSize = 20) : IRequest<PagedResult<EmployeeDto>>;
