using Serilog.Context;

namespace Batura.Finbridge.TestTask.Infrastructure.Logging;

/// <summary>
/// Контекст корреляции для распределённой трассировки и логирования
/// </summary>
/// <remarks>
/// Хранит идентификатор корреляции в AsyncLocal, что позволяет автоматически
/// передавать его по всей асинхронной цепочке вызовов в рамках текущего потока выполнения.
/// При установке идентификатора он также добавляется в контекст Serilog,
/// благодаря чему автоматически попадает во все последующие записи журнала.
/// </remarks>
public class CorrelationContext : ICorrelationContext
{
    private const string CORRELATION_ID_PROPERTY = "CorrelationId";

    private static readonly AsyncLocal<Guid> _correlationId = new();

    public IDisposable SetCorrelationId(Guid correlationId)
    {
        _correlationId.Value = correlationId;

        return LogContext.PushProperty(CORRELATION_ID_PROPERTY, correlationId);
    }

    public Guid CorrelationId => _correlationId.Value;
}