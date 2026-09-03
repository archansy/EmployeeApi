using EmployeeService.Domain.Entities;

namespace EmployeeService.Application.Interfaces;

public interface IRefreshTokenSessionRepository
{
    Task AddAsync(RefreshTokenSession session, CancellationToken cancellationToken);

    Task<RefreshTokenSession?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<RefreshTokenSession>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    Task RevokeAllByUserIdAsync(Guid userId, DateTime revokedAtUtc, string reason, CancellationToken cancellationToken);
}
