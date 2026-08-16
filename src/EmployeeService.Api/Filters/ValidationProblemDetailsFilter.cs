using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EmployeeService.Api.Filters;

public sealed class ValidationProblemDetailsFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is ObjectResult { Value: ValidationProblemDetails details })
        {
            details.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
            context.Result = new ObjectResult(details)
            {
                StatusCode = details.Status,
                ContentTypes = { "application/problem+json" }
            };
        }

        await next();
    }
}
