namespace Batura.Finbridge.TestTask.Application.IntegrationEvents;

/// <summary>
/// Событие интеграции
/// </summary>
public abstract class IntegrationEvent : IIntegrationEvent
{
    /// <summary>
    /// Конструктор класса IntegrationEvent
    /// </summary>
    /// <param name="entityId">Идентификатор сущности, связанной с событием</param>
    public IntegrationEvent(Guid entityId)
    {
        EventId = Guid.NewGuid();
        EntityId = entityId;
        CreatedOnUtc = DateTime.UtcNow;
    }

    /// <inheritdoc />
    public Guid EventId { get; init; }

    /// <inheritdoc />
    public Guid EntityId { get; init; }

    /// <inheritdoc />
    public DateTime CreatedOnUtc { get; init; }
}