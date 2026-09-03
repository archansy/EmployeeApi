namespace EmployeeService.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}
