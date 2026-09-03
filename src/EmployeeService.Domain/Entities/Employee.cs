using EmployeeService.Domain.Common;
using EmployeeService.Domain.Events;
using EmployeeService.Domain.ValueObjects;

namespace EmployeeService.Domain.Entities;

public sealed class Employee : BaseEntity
{
    private Employee(
        Guid id,
        string employeeNumber,
        string firstName,
        string lastName,
        Email email,
        string department,
        string jobTitle,
        DateOnly dateOfJoining,
        bool isActive,
        DateTime createdAtUtc,
        DateTime updatedAtUtc)
        : base(id)
    {
        EmployeeNumber = employeeNumber;
        FirstName = firstName;
        LastName = lastName;
        Email = email.Value;
        Department = department;
        JobTitle = jobTitle;
        DateOfJoining = dateOfJoining;
        IsActive = isActive;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
    }

    private Employee() : base(Guid.Empty)
    {
    }

    public string EmployeeNumber { get; private set; } = string.Empty;

    public string FirstName { get; private set; } = string.Empty;

    public string LastName { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string Department { get; private set; } = string.Empty;

    public string JobTitle { get; private set; } = string.Empty;

    public DateOnly DateOfJoining { get; private set; }

    public bool IsActive { get; private set; }

    public static Employee Create(
        string employeeNumber,
        string firstName,
        string lastName,
        string email,
        string department,
        string jobTitle,
        DateOnly dateOfJoining,
        DateTime utcNow)
    {
        var employee = new Employee(
            Guid.NewGuid(),
            employeeNumber.Trim(),
            firstName.Trim(),
            lastName.Trim(),
            ValueObjects.Email.Create(email),
            department.Trim(),
            jobTitle.Trim(),
            dateOfJoining,
            true,
            utcNow,
            utcNow);

        employee.AddDomainEvent(new EmployeeCreatedEvent(employee.Id));
        return employee;
    }

    public void Update(
        string firstName,
        string lastName,
        string email,
        string department,
        string jobTitle,
        DateOnly dateOfJoining,
        bool isActive,
        DateTime utcNow)
    {
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = ValueObjects.Email.Create(email).Value;
        Department = department.Trim();
        JobTitle = jobTitle.Trim();
        DateOfJoining = dateOfJoining;
        IsActive = isActive;
        UpdatedAtUtc = utcNow;
        AddDomainEvent(new EmployeeUpdatedEvent(Id));
    }

    public void Deactivate(DateTime utcNow)
    {
        IsActive = false;
        UpdatedAtUtc = utcNow;
        AddDomainEvent(new EmployeeDeletedEvent(Id));
    }
}
