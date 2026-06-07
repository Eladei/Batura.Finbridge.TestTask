namespace Batura.Finbridge.TestTask.Infrastructure.Outbox;

/// <summary>
/// Настройки для отправки событий интеграции из Outbox
/// </summary>
public sealed class OutboxOptions
{
    /// <summary>
    /// Время резервирования событий для отправки в секундах
    /// </summary>
    public uint ReservingSpanSeconds { get; set; }

    /// <summary>
    /// Максимальное количество событий для резервирования за один раз
    /// </summary>
    public uint MaxEventsToReserve { get; set; }
}