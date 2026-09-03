namespace EmployeeService.Application.Interfaces;

public interface IAuthAuditLogger
{
    void LoginSucceeded(Guid userId, string email, string? ipAddress);

    void LoginFailed(string email, string? ipAddress, string reason);

    void RefreshRotated(Guid userId, string? ipAddress);

    void RefreshReuseDetected(Guid userId, string? ipAddress);

    void Logout(Guid userId, bool allSessions, string? ipAddress);
}
