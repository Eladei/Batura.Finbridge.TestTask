namespace Batura.Finbridge.TestTask.Api.Middleware;

/// <summary>
/// Middleware для логирования входящих запросов
/// </summary>
public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    /// <summary>
    /// Создает объект класса <see cref="RequestLoggingMiddleware"/>
    /// </summary>
    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation(
            "Request started. Method: {Method}, Path: {Path}",
            context.Request.Method,
            context.Request.Path);

        await _next(context);

        _logger.LogInformation(
            "Request finished. StatusCode: {StatusCode}",
            context.Response.StatusCode);
    }
}