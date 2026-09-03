using EmployeeService.Application.DTOs;
using EmployeeService.Application.Exceptions;
using EmployeeService.Application.Interfaces;
using Mapster;
using MediatR;

namespace EmployeeService.Application.Features.Employees.Queries.GetEmployeeById;

public sealed class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, EmployeeDto>
{
    private readonly IEmployeeRepository _employeeRepository;

    public GetEmployeeByIdQueryHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<EmployeeDto> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Employee with id '{request.Id}' was not found.");

        return employee.Adapt<EmployeeDto>();
    }
}
