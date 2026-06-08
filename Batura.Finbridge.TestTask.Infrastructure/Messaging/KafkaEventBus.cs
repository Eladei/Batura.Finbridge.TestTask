using Batura.Finbridge.TestTask.Application.IntegrationEvents;
using Batura.Finbridge.TestTask.Infrastructure.Properties;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Batura.Finbridge.TestTask.Infrastructure.Messaging;

/// <summary>
/// Шина для публикации событий интеграции в Kafka
/// </summary>
public sealed class KafkaEventBus : IIntegrationEventBus
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaEventBus>? _logger;
    private readonly string _topic;

    /// <summary>
    /// Создает объект класса <see cref="KafkaEventBus"/>
    /// </summary>
    /// <param name="producer">Отправитель сообщений</param>
    /// <param name="topic">Топик, в который будет отправлено событие интеграции</param>
    /// <param name="logger">Логгер</param>
    public KafkaEventBus(IProducer<string, string> producer, string topic, ILogger<KafkaEventBus>? logger = null)
    {
        ArgumentNullException.ThrowIfNull(producer, nameof(producer));
        ArgumentNullException.ThrowIfNullOrWhiteSpace(topic, nameof(topic));

        _producer = producer;
        _topic = topic;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task PublishEventAsync(IIntegrationEvent integrationEvent)
    {
        try
        {
            var serializedEvent = JsonSerializer.Serialize(integrationEvent, integrationEvent.GetType());
            var message = new Message<string, string>
            {
                Key = integrationEvent.EntityId.ToString(),
                Value = serializedEvent
            };

            await _producer.ProduceAsync(_topic, message);

            if (_logger is not null)
            {
                var msg = string.Format(
                    Resources.IntegrationEventWasPublished,
                    integrationEvent.GetType().Name,
                    integrationEvent.EventId,
                    _topic);

                _logger.LogInformation(msg);
            }
        }
        catch (Exception ex)
        {
            var msg = string.Format(
                Resources.IntegrationEventPublishingError,
                integrationEvent.GetType().Name,
                integrationEvent.EventId,
                _topic);

            throw new EventPublishingException(msg, ex);
        }
    }
}