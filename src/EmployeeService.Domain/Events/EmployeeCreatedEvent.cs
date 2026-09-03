using EmployeeService.Domain.Common;

namespace EmployeeService.Domain.Events;

public sealed record EmployeeCreatedEvent(Guid EmployeeId) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
