using EmployeeService.Application.DTOs;
using EmployeeService.Application.Exceptions;
using EmployeeService.Application.Interfaces;
using EmployeeService.Domain.Entities;
using Mapster;
using MediatR;

namespace EmployeeService.Application.Features.Employees.Commands.CreateEmployee;

public sealed class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, EmployeeDto>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateEmployeeCommandHandler(
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<EmployeeDto> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        if (await _employeeRepository.ExistsByEmployeeNumberAsync(request.EmployeeNumber, null, cancellationToken))
        {
            throw new ConflictException("EmployeeNumber already exists.");
        }

        if (await _employeeRepository.ExistsByEmailAsync(request.Email, null, cancellationToken))
        {
            throw new ConflictException("Email already exists.");
        }

        var employee = Employee.Create(
            request.EmployeeNumber,
            request.FirstName,
            request.LastName,
            request.Email,
            request.Department,
            request.JobTitle,
            request.DateOfJoining,
            _dateTimeProvider.UtcNow);

        await _employeeRepository.AddAsync(employee, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return employee.Adapt<EmployeeDto>();
    }
}
