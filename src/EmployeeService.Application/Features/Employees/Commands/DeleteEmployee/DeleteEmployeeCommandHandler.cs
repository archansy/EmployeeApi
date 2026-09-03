using EmployeeService.Application.Exceptions;
using EmployeeService.Application.Interfaces;
using MediatR;

namespace EmployeeService.Application.Features.Employees.Commands.DeleteEmployee;

public sealed class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEmployeeCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Employee with id '{request.Id}' was not found.");

        _employeeRepository.Remove(employee);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
