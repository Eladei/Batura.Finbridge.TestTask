using Batura.Finbridge.TestTask.Api.Properties;
using Batura.Finbridge.TestTask.Infrastructure.Logging;

namespace Batura.Finbridge.TestTask.Api.Middleware;

/// <summary>
/// Middleware для логирования входящих запросов
/// </summary>
public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ICorrelationContext _correlationContext;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    /// <summary>
    /// Создает объект класса <see cref="RequestLoggingMiddleware"/>
    /// </summary>
    /// <param name="next">Делегат запроса</param>
    /// <param name="correlationContext">Контекст корреляции</param>
    /// <param name="logger">Опциональный логгер</param>
    /// <exception cref="ArgumentNullException"></exception>
    public RequestLoggingMiddleware(
        RequestDelegate next,
        ICorrelationContext correlationContext,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _correlationContext = correlationContext
            ?? throw new ArgumentNullException(nameof(correlationContext));

        _logger = logger;

        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        using (_correlationContext.SetCorrelationId(Guid.NewGuid()))
        {
            _logger?.LogInformation(
                Resources.RequestStarted,
                context.Request.Method,
                context.Request.Path);

            await _next(context);

            _logger?.LogInformation(
                Resources.RequestFinished,
                context.Response.StatusCode);
        }
    }
}