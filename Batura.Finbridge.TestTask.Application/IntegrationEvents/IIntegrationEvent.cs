namespace Batura.Finbridge.TestTask.Application.IntegrationEvents;

/// <summary>
/// Интеграционное событие
/// </summary>
public interface IIntegrationEvent
{
    /// <summary>
    /// Идентификатор события
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// Идентификатор сущности, связанной с событием
    /// </summary>
    Guid EntityId { get; }

    /// <summary>
    /// Время создания события в UTC
    /// </summary>
    DateTime CreatedOnUtc { get; }
}