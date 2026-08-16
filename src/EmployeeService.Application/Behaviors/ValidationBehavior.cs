using EmployeeService.Application.Exceptions;
using FluentValidation;
using MediatR;

namespace EmployeeService.Application.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var results = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var errors = results
            .Where(x => !x.IsValid)
            .SelectMany(x => x.Errors)
            .Where(x => x is not null)
            .GroupBy(
                x => x.PropertyName,
                x => x.ErrorMessage,
                StringComparer.OrdinalIgnoreCase)
            .ToDictionary(x => x.Key, x => x.Distinct().ToArray(), StringComparer.OrdinalIgnoreCase);

        if (errors.Count > 0)
        {
            throw new EmployeeService.Application.Exceptions.ValidationException(errors);
        }

        return await next();
    }
}
