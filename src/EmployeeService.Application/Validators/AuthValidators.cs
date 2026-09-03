using EmployeeService.Application.DTOs;
using FluentValidation;

namespace EmployeeService.Application.Validators;

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
    }
}

public sealed class RefreshRequestValidator : AbstractValidator<RefreshRequest>
{
    public RefreshRequestValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}

public sealed class LogoutRequestValidator : AbstractValidator<LogoutRequest>
{
    public LogoutRequestValidator()
    {
        RuleFor(x => x)
            .Must(x => x.LogoutAllSessions || !string.IsNullOrWhiteSpace(x.RefreshToken))
            .WithMessage("Either refreshToken or logoutAllSessions=true must be provided.");
    }
}
