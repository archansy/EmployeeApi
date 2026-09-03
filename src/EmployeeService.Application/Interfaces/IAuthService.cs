using System.Security.Claims;
using EmployeeService.Application.DTOs;

namespace EmployeeService.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, string? ipAddress, string? userAgent, CancellationToken cancellationToken);

    Task<AuthUserDto> GetCurrentUserAsync(ClaimsPrincipal principal, CancellationToken cancellationToken);

    Task<RefreshResponse> RefreshAsync(RefreshRequest request, string? ipAddress, string? userAgent, CancellationToken cancellationToken);

    Task LogoutAsync(ClaimsPrincipal principal, LogoutRequest request, CancellationToken cancellationToken);
}
