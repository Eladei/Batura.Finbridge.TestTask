using Batura.Finbridge.TestTask.Application.Commands;
using Batura.Finbridge.TestTask.Model;
using Microsoft.EntityFrameworkCore;

namespace Batura.Finbridge.TestTask.Infrastructure.Outbox;

/// <summary>
/// Команда для резервирования событий интеграции в Outbox для последующей отправки
/// </summary>
public sealed class ReserveIntegrationEventsInOutboxForSendingCommand : CommandBase<UserDbContext>
{
    private readonly Guid _senderId;
    private readonly uint _reservingSpanSeconds;
    private readonly uint _maxEventsToReserve;

    /// <summary>
    /// Создает объект класса <see cref="ReserveIntegrationEventsInOutboxForSendingCommand"/>
    /// </summary>
    /// <param name="senderId">Идентификатор сервиса, отправляющего события</param>
    /// <param name="reservingSpanSeconds">Время резервирования в секундах</param>
    /// <param name="maxEventsToReserve">Максимальное количество событий для резервирования</param>
    public ReserveIntegrationEventsInOutboxForSendingCommand(Guid senderId, uint reservingSpanSeconds, uint maxEventsToReserve)
    {
        _senderId = senderId;
        _reservingSpanSeconds = reservingSpanSeconds;
        _maxEventsToReserve = maxEventsToReserve;
    }

    /// <inheritdoc />
    public override async Task ExecuteAsync(UserDbContext context, CancellationToken cancellationToken)
    {
        var reservingDate = DateTime.UtcNow;

        var eventsToReserve = await context.IntegrationEvents
            .Where(x => !x.IsSent
                && (x.ReservedAt == null
                    || reservingDate >= x.ReservedAt.Value.AddSeconds(_reservingSpanSeconds)))
            .OrderBy(x => x.CreatedAtUtc)
            .Take((int)_maxEventsToReserve)
            .ToArrayAsync(cancellationToken);

        foreach (var evnt in eventsToReserve)
        {
            evnt.ReservedAt = reservingDate;
            evnt.ReservedBy = _senderId;
        }
    }
}