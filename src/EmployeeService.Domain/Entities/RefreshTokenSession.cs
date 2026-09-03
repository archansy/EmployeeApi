namespace EmployeeService.Domain.Entities;

public sealed class RefreshTokenSession
{
    private RefreshTokenSession()
    {
    }

    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public string RefreshTokenHash { get; private set; } = string.Empty;

    public string? ReplacedByTokenHash { get; private set; }

    public string? DeviceInfo { get; private set; }

    public string? IpAddress { get; private set; }

    public DateTime ExpiresAtUtc { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? RevokedAtUtc { get; private set; }

    public string? RevokedReason { get; private set; }

    public DateTime? ReuseDetectedAtUtc { get; private set; }

    public User User { get; private set; } = null!;

    public static RefreshTokenSession Create(
        Guid userId,
        string refreshTokenHash,
        string? deviceInfo,
        string? ipAddress,
        DateTime createdAtUtc,
        DateTime expiresAtUtc)
    {
        return new RefreshTokenSession
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RefreshTokenHash = refreshTokenHash,
            DeviceInfo = deviceInfo,
            IpAddress = ipAddress,
            CreatedAtUtc = createdAtUtc,
            ExpiresAtUtc = expiresAtUtc
        };
    }

    public bool IsExpired(DateTime utcNow) => ExpiresAtUtc <= utcNow;

    public bool IsRevoked => RevokedAtUtc.HasValue;

    public void Revoke(DateTime utcNow, string reason, string? replacedByTokenHash = null)
    {
        RevokedAtUtc = utcNow;
        RevokedReason = reason;
        ReplacedByTokenHash = replacedByTokenHash;
    }

    public void MarkReuseDetected(DateTime utcNow)
    {
        ReuseDetectedAtUtc = utcNow;
    }
}
