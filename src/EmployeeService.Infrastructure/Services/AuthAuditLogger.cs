using EmployeeService.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace EmployeeService.Infrastructure.Services;

public sealed class AuthAuditLogger : IAuthAuditLogger
{
    private readonly ILogger<AuthAuditLogger> _logger;

    public AuthAuditLogger(ILogger<AuthAuditLogger> logger)
    {
        _logger = logger;
    }

    public void LoginSucceeded(Guid userId, string email, string? ipAddress)
    {
        _logger.LogInformation("Audit login_succeeded userId={UserId} email={Email} ip={IpAddress}", userId, email, ipAddress);
    }

    public void LoginFailed(string email, string? ipAddress, string reason)
    {
        _logger.LogWarning("Audit login_failed email={Email} ip={IpAddress} reason={Reason}", email, ipAddress, reason);
    }

    public void RefreshRotated(Guid userId, string? ipAddress)
    {
        _logger.LogInformation("Audit refresh_rotated userId={UserId} ip={IpAddress}", userId, ipAddress);
    }

    public void RefreshReuseDetected(Guid userId, string? ipAddress)
    {
        _logger.LogWarning("Audit refresh_reuse_detected userId={UserId} ip={IpAddress}", userId, ipAddress);
    }

    public void Logout(Guid userId, bool allSessions, string? ipAddress)
    {
        _logger.LogInformation("Audit logout userId={UserId} allSessions={AllSessions} ip={IpAddress}", userId, allSessions, ipAddress);
    }
}
