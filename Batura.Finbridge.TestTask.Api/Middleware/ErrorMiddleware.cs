using Batura.Finbridge.TestTask.Infrastructure.Logging;
using Microsoft.EntityFrameworkCore;

namespace Batura.Finbridge.TestTask.Api.Middleware;

/// <summary>
/// Middleware для обработки ошибок
/// </summary>
public sealed class ErrorMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ICorrelationContext _correlationContext;
    private readonly ILogger<ErrorMiddleware> _logger;

    /// <summary>
    /// Создает объект класса <see cref="RequestLoggingMiddleware"/>
    /// </summary>
    /// <param name="next">Делегат запроса</param>
    /// <param name="correlationContext">Контекст корреляции</param>
    /// <param name="logger">Опциональный логгер</param>
    /// <exception cref="ArgumentNullException"></exception>
    public ErrorMiddleware(
        RequestDelegate next,
        ICorrelationContext correlationContext,
        ILogger<ErrorMiddleware> logger)
    {
        _correlationContext = correlationContext
            ?? throw new ArgumentNullException(nameof(correlationContext));

        _logger = logger;

        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var exception = ex.InnerException ?? ex;

        _logger.LogError(exception,
            "Error in {Method} with CorrelationId {CorrelationId}",
            context.Request.Method,
            _correlationContext.CorrelationId);

        var (statusCode, message) = MapException(exception);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(new
        {
            error = message,
            correlationId = _correlationContext.CorrelationId
        });
    }

    private static (int statusCode, string message) MapException(Exception ex)
        => ex switch
        {
            OperationCanceledException => (499, "Request cancelled"),
            TimeoutException => (504, "Timeout"),
            ArgumentException => (400, ex.Message),
            InvalidOperationException => (400, ex.Message),
            DbUpdateException => (409, "Database error"),
            _ => (500, "Internal server error")
        };
}