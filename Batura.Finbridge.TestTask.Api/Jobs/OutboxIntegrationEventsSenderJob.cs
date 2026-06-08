using Batura.Finbridge.TestTask.Application.Commands;
using Batura.Finbridge.TestTask.Infrastructure.Logging;
using Batura.Finbridge.TestTask.Infrastructure.Messaging;
using Batura.Finbridge.TestTask.Infrastructure.Outbox;
using Batura.Finbridge.TestTask.Model;
using Microsoft.Extensions.Options;

namespace Batura.Finbridge.TestTask.Api.Jobs;

/// <summary>
/// Задание для отправки интеграционных событий из outbox
/// </summary>
/// <remarks>
/// Резервирует интеграционные события в outbox и публикует их.
/// Зарезервированные события становятся доступны для повторного резервирования в следующих случаях:
/// - событие успешно отправлено;
/// - при отправке произошла ошибка;
/// - истёк срок резервирования.
/// </remarks>
public sealed class OutboxIntegrationEventsSenderJob : QuartzJobBase
{
    private readonly OutboxOptions _jobConfig;
    private readonly Guid _senderId;
    private readonly ICommandExecutor<UserDbContext> _commandExecutor;
    private readonly IIntegrationEventBus _integrationEventBus;

    /// <summary>
    /// Задание, отвечающее за отправку интеграционных событий из outbox
    /// </summary>
    /// <param name="jobConfig">Настройки задания отправки интеграционных событий из outbox</param>
    /// <param name="commandExecutor">Исполнитель команд</param>
    /// <param name="integrationEventBus">Шина интеграционных событий для их публикации</param>
    /// <param name="correlationContext">Контекст корреляции для распределённой трассировки и логирования</param>
    /// <param name="logger">Экземпляр логгера</param>
    /// <exception cref="ArgumentNullException"></exception>
    public OutboxIntegrationEventsSenderJob(
        IOptions<OutboxOptions> jobConfig,
        ICommandExecutor<UserDbContext> commandExecutor,
        IIntegrationEventBus integrationEventBus,
        ICorrelationContext correlationContext,
        ILogger<OutboxIntegrationEventsSenderJob> logger) : base(correlationContext, logger)
    {
        _jobConfig = jobConfig?.Value
            ?? throw new ArgumentNullException(nameof(jobConfig));

        _commandExecutor = commandExecutor
            ?? throw new ArgumentNullException(nameof(commandExecutor));

        _integrationEventBus = integrationEventBus
            ?? throw new ArgumentNullException(nameof(integrationEventBus));

        _senderId = Guid.NewGuid();
    }

    /// <inheritdoc />
    protected override async Task Perform(CancellationToken cancellationToken)
    {
        await _commandExecutor.ExecuteAsync(
            new ReserveIntegrationEventsInOutboxForSendingCommand(
                _senderId, _jobConfig.ReservingSpanSeconds, _jobConfig.MaxEventsToReserve), cancellationToken);

        await _commandExecutor.ExecuteAsync(
            new SendIntegrationEventsFromOutboxCommand(
                _senderId, _jobConfig.ReservingSpanSeconds, _integrationEventBus), cancellationToken);
    }
}