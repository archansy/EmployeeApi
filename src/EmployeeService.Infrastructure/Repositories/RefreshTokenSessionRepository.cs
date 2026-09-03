using EmployeeService.Application.Interfaces;
using EmployeeService.Domain.Entities;
using EmployeeService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EmployeeService.Infrastructure.Repositories;

public sealed class RefreshTokenSessionRepository : IRefreshTokenSessionRepository
{
    private readonly ApplicationDbContext _dbContext;

    public RefreshTokenSessionRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(RefreshTokenSession session, CancellationToken cancellationToken)
    {
        await _dbContext.RefreshTokenSessions.AddAsync(session, cancellationToken);
    }

    public async Task<RefreshTokenSession?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken)
    {
        return await _dbContext.RefreshTokenSessions
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.RefreshTokenHash == tokenHash, cancellationToken);
    }

    public async Task<IReadOnlyCollection<RefreshTokenSession>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _dbContext.RefreshTokenSessions
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task RevokeAllByUserIdAsync(Guid userId, DateTime revokedAtUtc, string reason, CancellationToken cancellationToken)
    {
        var sessions = await _dbContext.RefreshTokenSessions
            .Where(x => x.UserId == userId && x.RevokedAtUtc == null)
            .ToListAsync(cancellationToken);

        foreach (var session in sessions)
        {
            session.Revoke(revokedAtUtc, reason);
        }
    }
}
