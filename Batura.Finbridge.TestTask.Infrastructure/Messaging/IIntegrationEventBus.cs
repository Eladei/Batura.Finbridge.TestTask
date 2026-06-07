using Batura.Finbridge.TestTask.Application.IntegrationEvents;

namespace Batura.Finbridge.TestTask.Infrastructure.Messaging;

/// <summary>
/// Шина для публикации событий интеграции
/// </summary>
public interface IIntegrationEventBus
{
    /// <summary>
    /// Публикует в шину события интеграции
    /// </summary>
    /// <param name="integrationEvent">Событие интеграции</param>
    Task PublishEventAsync(IIntegrationEvent integrationEvent);
}