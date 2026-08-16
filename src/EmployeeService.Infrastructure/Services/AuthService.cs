using System.Security.Claims;
using EmployeeService.Application.DTOs;
using EmployeeService.Application.Exceptions;
using EmployeeService.Application.Interfaces;
using EmployeeService.Domain.Entities;
using EmployeeService.Domain.Enums;
using Microsoft.Extensions.Configuration;

namespace EmployeeService.Infrastructure.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenSessionRepository _refreshTokenSessionRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthAuditLogger _auditLogger;
    private readonly int _lockoutThreshold;
    private readonly int _lockoutMinutes;
    private readonly int _refreshTokenDays;

    public AuthService(
        IUserRepository userRepository,
        IRefreshTokenSessionRepository refreshTokenSessionRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork,
        IAuthAuditLogger auditLogger,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _refreshTokenSessionRepository = refreshTokenSessionRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
        _auditLogger = auditLogger;
        _lockoutThreshold = int.TryParse(configuration["Auth:LockoutThreshold"], out var threshold) ? threshold : 5;
        _lockoutMinutes = int.TryParse(configuration["Auth:LockoutMinutes"], out var minutes) ? minutes : 15;
        _refreshTokenDays = int.TryParse(configuration["Auth:RefreshTokenExpiryDays"], out var days) ? days : 7;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, string? ipAddress, string? userAgent, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

        if (user is null)
        {
            _auditLogger.LoginFailed(email, ipAddress, "user_not_found");
            throw new UnauthorizedException("Invalid credentials.");
        }

        if (user.Status != UserStatus.Active)
        {
            _auditLogger.LoginFailed(email, ipAddress, "inactive_user");
            throw new UnauthorizedException("User account is inactive.");
        }

        var now = _dateTimeProvider.UtcNow;
        if (user.IsLockedOut(now))
        {
            _auditLogger.LoginFailed(email, ipAddress, "locked_out");
            throw new UnauthorizedException("Login temporarily locked due to failed attempts.");
        }

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            user.IncrementFailedAttempts(now, threshold: _lockoutThreshold, lockoutDuration: TimeSpan.FromMinutes(_lockoutMinutes));
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _auditLogger.LoginFailed(email, ipAddress, "invalid_password");
            throw new UnauthorizedException("Invalid credentials.");
        }

        user.ResetFailedAttempts(now);

        var (accessToken, expiresIn) = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();
        var refreshTokenHash = _tokenService.HashRefreshToken(refreshToken);

        var session = RefreshTokenSession.Create(
            user.Id,
            refreshTokenHash,
            userAgent,
            ipAddress,
            now,
            now.AddDays(_refreshTokenDays));

        await _refreshTokenSessionRepository.AddAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _auditLogger.LoginSucceeded(user.Id, user.Email, ipAddress);

        return new LoginResponse
        {
            User = new AuthUserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString().ToLowerInvariant(),
                Status = user.Status.ToString().ToLowerInvariant()
            },
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = expiresIn
        };
    }

    public async Task<AuthUserDto> GetCurrentUserAsync(ClaimsPrincipal principal, CancellationToken cancellationToken)
    {
        var sub = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? principal.FindFirst("sub")?.Value;

        if (!Guid.TryParse(sub, out var userId))
        {
            throw new UnauthorizedException("Invalid token.");
        }

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new UnauthorizedException("User not found.");

        return new AuthUserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role.ToString().ToLowerInvariant(),
            Status = user.Status.ToString().ToLowerInvariant()
        };
    }

    public async Task<RefreshResponse> RefreshAsync(RefreshRequest request, string? ipAddress, string? userAgent, CancellationToken cancellationToken)
    {
        var now = _dateTimeProvider.UtcNow;
        var tokenHash = _tokenService.HashRefreshToken(request.RefreshToken);
        var session = await _refreshTokenSessionRepository.GetByTokenHashAsync(tokenHash, cancellationToken);

        if (session is null)
        {
            throw new UnauthorizedException("Invalid refresh token.");
        }

        if (session.IsRevoked)
        {
            session.MarkReuseDetected(now);
            await _refreshTokenSessionRepository.RevokeAllByUserIdAsync(session.UserId, now, "refresh_token_reuse_detected", cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _auditLogger.RefreshReuseDetected(session.UserId, ipAddress);
            throw new UnauthorizedException("Refresh token is revoked.");
        }

        if (session.IsExpired(now))
        {
            session.Revoke(now, "refresh_token_expired");
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            throw new UnauthorizedException("Refresh token expired.");
        }

        var user = session.User;
        var (accessToken, expiresIn) = _tokenService.GenerateAccessToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        var newRefreshHash = _tokenService.HashRefreshToken(newRefreshToken);

        session.Revoke(now, "rotated", newRefreshHash);

        var newSession = RefreshTokenSession.Create(
            user.Id,
            newRefreshHash,
            userAgent,
            ipAddress,
            now,
            now.AddDays(_refreshTokenDays));

        await _refreshTokenSessionRepository.AddAsync(newSession, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _auditLogger.RefreshRotated(user.Id, ipAddress);

        return new RefreshResponse
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            ExpiresIn = expiresIn
        };
    }

    public async Task LogoutAsync(ClaimsPrincipal principal, LogoutRequest request, CancellationToken cancellationToken)
    {
        var sub = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? principal.FindFirst("sub")?.Value;

        if (!Guid.TryParse(sub, out var userId))
        {
            throw new UnauthorizedException("Invalid token.");
        }

        var now = _dateTimeProvider.UtcNow;

        if (request.LogoutAllSessions)
        {
            await _refreshTokenSessionRepository.RevokeAllByUserIdAsync(userId, now, "logout_all", cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _auditLogger.Logout(userId, allSessions: true, ipAddress: null);
            return;
        }

        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            throw new UnauthorizedException("Refresh token is required.");
        }

        var tokenHash = _tokenService.HashRefreshToken(request.RefreshToken);
        var session = await _refreshTokenSessionRepository.GetByTokenHashAsync(tokenHash, cancellationToken);

        if (session is null || session.UserId != userId)
        {
            throw new UnauthorizedException("Invalid refresh token.");
        }

        if (!session.IsRevoked)
        {
            session.Revoke(now, "logout");
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        _auditLogger.Logout(userId, allSessions: false, ipAddress: null);
    }
}
