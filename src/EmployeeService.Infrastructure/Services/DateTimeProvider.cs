using EmployeeService.Application.Interfaces;

namespace EmployeeService.Infrastructure.Services;

public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
