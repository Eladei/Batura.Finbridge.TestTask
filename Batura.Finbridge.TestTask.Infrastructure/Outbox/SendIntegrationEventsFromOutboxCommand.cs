using Batura.Finbridge.TestTask.Application.Commands;
using Batura.Finbridge.TestTask.Application.IntegrationEvents;
using Batura.Finbridge.TestTask.Infrastructure.Messaging;
using Batura.Finbridge.TestTask.Model;
using Batura.Finbridge.TestTask.Model.Entities.Outbox;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Batura.Finbridge.TestTask.Infrastructure.Outbox;

/// <summary>
/// Команда для отправки событий интеграции из Outbox
/// </summary>
public sealed class SendIntegrationEventsFromOutboxCommand : CommandBase<UserDbContext>
{
    private readonly Guid _senderId;
    private readonly uint _reservingSpanSeconds;
    private readonly IIntegrationEventBus _integrationEventBus;

    /// <summary>
    /// Создает объект класса <see cref="SendIntegrationEventsFromOutboxCommand"/>
    /// </summary>
    /// <param name="senderId">Идентификатор сервиса, отправляющего события</param>
    /// <param name="reservingSpanSeconds">Время резервирования в секундах</param>
    /// <param name="integrationEventBus">Шина событий интеграции</param>
    /// <exception cref="ArgumentNullException"></exception>
    public SendIntegrationEventsFromOutboxCommand(
        Guid senderId,
        uint reservingSpanSeconds,
        IIntegrationEventBus integrationEventBus)
    {
        _senderId = senderId;
        _reservingSpanSeconds = reservingSpanSeconds;

        _integrationEventBus = integrationEventBus
            ?? throw new ArgumentNullException(nameof(integrationEventBus));
    }

    /// <inheritdoc />
    public override async Task ExecuteAsync(UserDbContext context, CancellationToken cancellationToken)
    {
        var sendingDate = DateTime.UtcNow;

        var eventsToSend = await context.IntegrationEvents
            .Where(x => !x.IsSent
                && x.ReservedBy == _senderId
                && x.ReservedAt != null
                && sendingDate < x.ReservedAt.Value.AddSeconds(_reservingSpanSeconds))
            .OrderBy(x => x.CreatedAtUtc)
            .ToArrayAsync(cancellationToken);

        var sendEvents = true;

        // Отправляем события в шину по одному.
        // При возникновении ошибки при отправке, сохраняем информацию об ошибке в БД и не отправляем следующие события.
        foreach (var evnt in eventsToSend)
        {
            try
            {
                evnt.NumberOfSendingAttempts += 1;

                var integrationEvent = Map(evnt);

                if (sendEvents)
                {
                    await _integrationEventBus.PublishEventAsync(integrationEvent);
                    evnt.IsSent = true;
                    evnt.SentAt = sendingDate;
                }
            }
            catch (Exception ex)
            {
                evnt.LastError = ex.Message;
                sendEvents = false;
            }

            evnt.ReservedBy = null;
        }
    }

    private static IIntegrationEvent Map(IntegrationEventToSend eventDb)
    {
        var eventType = Type.GetType(eventDb.EventType)
            ?? throw new Exception("Type resolution error");

        var result = JsonSerializer.Deserialize(eventDb.EventMetadata, eventType)
            ?? throw new Exception("Deserialization error");

        return (IIntegrationEvent)result;
    }
}