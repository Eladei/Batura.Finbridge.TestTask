using System.ComponentModel.DataAnnotations;

namespace Batura.Finbridge.TestTask.Model.Entities.Outbox;

/// <summary>
/// Информация об отправке интеграционного события
/// </summary>
public class IntegrationEventToSend : EntityBase
{
    /// <summary>
    /// Идентификатор события
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор сущности, связанной с событием
    /// </summary>
    public Guid EntityId { get; set; }

    /// <summary>
    /// Идентификатор для распределённой трассировки
    /// </summary>
    public Guid CorrelationId { get; set; }

    /// <summary>
    /// Тип события
    /// </summary>
    [Required]
    public string EventType { get; set; } = null!;

    /// <summary>
    /// Метаданные события для его обработки
    /// </summary>
    [Required]
    public string EventMetadata { get; set; } = null!;

    /// <summary>
    /// Признак успешной отправки
    /// </summary>
    public bool IsSent { get; set; }

    /// <summary>
    /// Количество попыток отправки
    /// </summary>
    public int NumberOfSendingAttempts { get; set; }

    /// <summary>
    /// Дата отправки
    /// </summary>
    public DateTime? SentAt { get; set; }

    /// <summary>
    /// Последняя ошибка при отправке
    /// </summary>
    public string? LastError { get; set; }

    /// <summary>
    /// Идентификатор системы, зарезервировавшей событие для отправки
    /// </summary>
    public Guid? ReservedBy { get; set; }

    /// <summary>
    /// Дата резервирования события для отправки
    /// </summary>
    public DateTime? ReservedAt { get; set; }
}