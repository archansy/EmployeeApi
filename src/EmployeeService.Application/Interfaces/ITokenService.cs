using EmployeeService.Domain.Entities;

namespace EmployeeService.Application.Interfaces;

public interface ITokenService
{
    (string AccessToken, int ExpiresInSeconds) GenerateAccessToken(User user);

    string GenerateRefreshToken();

    string HashRefreshToken(string refreshToken);
}
