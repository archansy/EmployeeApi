using EmployeeService.Domain.Exceptions;

namespace EmployeeService.Domain.ValueObjects;

public sealed record Email
{
    private Email(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !value.Contains('@'))
        {
            throw new DomainException("Email is invalid.");
        }

        return new Email(value.Trim().ToLowerInvariant());
    }

    public override string ToString() => Value;
}
