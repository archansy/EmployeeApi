using EmployeeService.Application.DTOs;
using EmployeeService.Application.Exceptions;
using EmployeeService.Application.Interfaces;
using Mapster;
using MediatR;

namespace EmployeeService.Application.Features.Employees.Commands.UpdateEmployee;

public sealed class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, EmployeeDto>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateEmployeeCommandHandler(
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<EmployeeDto> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Employee with id '{request.Id}' was not found.");

        if (!string.Equals(Convert.ToBase64String(employee.RowVersion), request.ConcurrencyToken, StringComparison.Ordinal))
        {
            throw new ConflictException("The employee was updated by another request. Refresh and retry.");
        }

        if (await _employeeRepository.ExistsByEmailAsync(request.Email, request.Id, cancellationToken))
        {
            throw new ConflictException("Email already exists.");
        }

        employee.Update(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Department,
            request.JobTitle,
            request.DateOfJoining,
            request.IsActive,
            _dateTimeProvider.UtcNow);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return employee.Adapt<EmployeeDto>();
    }
}
