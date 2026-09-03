using System.Text;
using System.Text.Json;

namespace EmployeeService.Api.Middleware;

public sealed class SensitiveRequestLoggingMiddleware : IMiddleware
{
    private static readonly HashSet<string> SensitiveKeys =
    [
        "password",
        "refreshToken",
        "accessToken",
        "token",
        "authorization"
    ];

    private readonly ILogger<SensitiveRequestLoggingMiddleware> _logger;

    public SensitiveRequestLoggingMiddleware(ILogger<SensitiveRequestLoggingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!HttpMethods.IsPost(context.Request.Method))
        {
            await next(context);
            return;
        }

        if (!context.Request.Path.StartsWithSegments("/api/v1/auth", StringComparison.OrdinalIgnoreCase))
        {
            await next(context);
            return;
        }

        context.Request.EnableBuffering();

        string bodyText;
        using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
        {
            bodyText = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
        }

        string maskedBody = bodyText;
        if (!string.IsNullOrWhiteSpace(bodyText))
        {
            try
            {
                using var document = JsonDocument.Parse(bodyText);
                var dict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

                foreach (var property in document.RootElement.EnumerateObject())
                {
                    dict[property.Name] = SensitiveKeys.Contains(property.Name)
                        ? "***"
                        : property.Value.ToString();
                }

                maskedBody = JsonSerializer.Serialize(dict);
            }
            catch
            {
                maskedBody = "{\"payload\":\"unparseable\"}";
            }
        }

        _logger.LogInformation("Auth request {Method} {Path} body={Body}", context.Request.Method, context.Request.Path, maskedBody);

        await next(context);
    }
}
