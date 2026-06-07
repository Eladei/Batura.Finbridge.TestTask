namespace Batura.Finbridge.TestTask.Infrastructure.Logging;

/// <summary>
/// Контекст корреляции
/// </summary>
/// <remarks>
/// Используется для передачи идентификатора корреляции по всей системе
/// с целью трассировки полного пути выполнения операции
/// </remarks>
public interface ICorrelationContext
{
    /// <summary>
    /// Идентификатор корреляции, используемый для сквозной трассировки
    /// </summary>
    Guid CorrelationId { get; }

    /// <summary>
    /// Устанавливает идентификатор корреляции для текущей области выполнения
    /// </summary>
    /// <param name="correlationId">Идентификатор корреляции</param>
    /// <returns>
    /// Объект, освобождение которого восстанавливает предыдущий контекст корреляции
    /// </returns>
    IDisposable SetCorrelationId(Guid correlationId);
}