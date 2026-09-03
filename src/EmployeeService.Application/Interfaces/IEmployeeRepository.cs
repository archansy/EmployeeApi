using EmployeeService.Domain.Entities;

namespace EmployeeService.Application.Interfaces;

public interface IEmployeeRepository
{
    Task AddAsync(Employee employee, CancellationToken cancellationToken);

    Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Employee>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<long> CountAsync(CancellationToken cancellationToken);

    Task<bool> ExistsByEmployeeNumberAsync(string employeeNumber, Guid? excludingId, CancellationToken cancellationToken);

    Task<bool> ExistsByEmailAsync(string email, Guid? excludingId, CancellationToken cancellationToken);

    void Remove(Employee employee);
}
