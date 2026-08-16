using EmployeeService.Domain.Common;
using EmployeeService.Domain.Enums;

namespace EmployeeService.Domain.Entities;

public sealed class User : BaseEntity
{
    private User() : base(Guid.Empty)
    {
    }

    public User(
        Guid id,
        string name,
        string email,
        string passwordHash,
        UserRole role,
        UserStatus status,
        DateTime createdAtUtc,
        DateTime updatedAtUtc)
        : base(id)
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        Status = status;
        FailedLoginAttempts = 0;
        LockoutEndUtc = null;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
    }

    public string Name { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;

    public UserRole Role { get; private set; }

    public UserStatus Status { get; private set; }

    public int FailedLoginAttempts { get; private set; }

    public DateTime? LockoutEndUtc { get; private set; }

    public bool IsLockedOut(DateTime utcNow)
    {
        return LockoutEndUtc.HasValue && LockoutEndUtc.Value > utcNow;
    }

    public void IncrementFailedAttempts(DateTime utcNow, int threshold, TimeSpan lockoutDuration)
    {
        FailedLoginAttempts++;

        if (FailedLoginAttempts >= threshold)
        {
            LockoutEndUtc = utcNow.Add(lockoutDuration);
            FailedLoginAttempts = 0;
        }

        UpdatedAtUtc = utcNow;
    }

    public void ResetFailedAttempts(DateTime utcNow)
    {
        FailedLoginAttempts = 0;
        LockoutEndUtc = null;
        UpdatedAtUtc = utcNow;
    }
}
