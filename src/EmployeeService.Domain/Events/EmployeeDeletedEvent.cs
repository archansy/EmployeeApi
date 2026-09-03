using EmployeeService.Domain.Common;

namespace EmployeeService.Domain.Events;

public sealed record EmployeeDeletedEvent(Guid EmployeeId) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
