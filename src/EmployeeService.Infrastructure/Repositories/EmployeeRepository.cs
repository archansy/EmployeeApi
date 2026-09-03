using EmployeeService.Application.Interfaces;
using EmployeeService.Domain.Entities;
using EmployeeService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EmployeeService.Infrastructure.Repositories;

public sealed class EmployeeRepository : IEmployeeRepository
{
    private readonly ApplicationDbContext _dbContext;

    public EmployeeRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Employee employee, CancellationToken cancellationToken)
    {
        await _dbContext.Employees.AddAsync(employee, cancellationToken);
    }

    public async Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Employees.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Employee>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        return await _dbContext.Employees
            .AsNoTracking()
            .OrderBy(x => x.EmployeeNumber)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<long> CountAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Employees.LongCountAsync(cancellationToken);
    }

    public async Task<bool> ExistsByEmployeeNumberAsync(string employeeNumber, Guid? excludingId, CancellationToken cancellationToken)
    {
        var query = _dbContext.Employees.AsNoTracking().Where(x => x.EmployeeNumber == employeeNumber);

        if (excludingId.HasValue)
        {
            query = query.Where(x => x.Id != excludingId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, Guid? excludingId, CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var query = _dbContext.Employees.AsNoTracking().Where(x => x.Email == normalizedEmail);

        if (excludingId.HasValue)
        {
            query = query.Where(x => x.Id != excludingId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public void Remove(Employee employee)
    {
        _dbContext.Employees.Remove(employee);
    }
}
