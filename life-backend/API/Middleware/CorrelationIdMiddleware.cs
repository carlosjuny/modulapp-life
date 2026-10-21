using Serilog.Context;

namespace API.Middleware;

public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    public const string HeaderName = "X-Correlation-ID";
    public const string ItemKey = "CorrelationId";
    private const int MaxLength = 64;

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = ResolveCorrelationId(context.Request.Headers[HeaderName].FirstOrDefault());

        context.Items[ItemKey] = correlationId;
        context.Response.Headers[HeaderName] = correlationId;

        SentrySdk.ConfigureScope(scope => scope.SetTag("correlation_id", correlationId));

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await next(context);
        }
    }

    private static string ResolveCorrelationId(string? incoming)
    {
        if (!string.IsNullOrWhiteSpace(incoming)
            && incoming.Length <= MaxLength
            && incoming.All(c => char.IsAsciiLetterOrDigit(c) || c is '-' or '_'))
        {
            return incoming;
        }

        return Guid.NewGuid().ToString("N");
    }
}
