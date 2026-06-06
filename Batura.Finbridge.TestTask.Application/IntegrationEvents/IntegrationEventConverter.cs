using Batura.Finbridge.TestTask.Model.Entities.Outbox;
using System.Text.Json;

namespace Batura.Finbridge.TestTask.Application.IntegrationEvents;

/// <summary>
/// Преобразует событие интеграции в событие для последующей отправки (Outbox)
/// </summary>
public static class IntegrationEventConverter
{
    /// <summary>
    /// Преобразует событие интеграции в событие для последующей отправки (Outbox)
    /// </summary>
    /// <param name="integrationEvent">Событие интеграции</param>
    /// <returns>Событие для отправки</returns>
    public static IntegrationEventToSend Convert(IIntegrationEvent integrationEvent)
    {
        var eventType = integrationEvent.GetType();

        var metadata = JsonSerializer.Serialize(integrationEvent, eventType);

        return new IntegrationEventToSend
        {
            Id = integrationEvent.EventId,
            EntityId = integrationEvent.EntityId,
            EventType = eventType.AssemblyQualifiedName!,
            EventMetadata = metadata
        };
    }
}