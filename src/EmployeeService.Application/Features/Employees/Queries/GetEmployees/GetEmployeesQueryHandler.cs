using EmployeeService.Application.DTOs;
using EmployeeService.Application.Interfaces;
using Mapster;
using MediatR;

namespace EmployeeService.Application.Features.Employees.Queries.GetEmployees;

public sealed class GetEmployeesQueryHandler : IRequestHandler<GetEmployeesQuery, PagedResult<EmployeeDto>>
{
    private readonly IEmployeeRepository _employeeRepository;

    public GetEmployeesQueryHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<PagedResult<EmployeeDto>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
    {
        var employees = await _employeeRepository.GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);
        var total = await _employeeRepository.CountAsync(cancellationToken);

        return new PagedResult<EmployeeDto>
        {
            Items = employees.Select(x => x.Adapt<EmployeeDto>()).ToArray(),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = total
        };
    }
}
