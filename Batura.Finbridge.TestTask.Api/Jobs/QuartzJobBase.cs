using Batura.Finbridge.TestTask.Api.Properties;
using Batura.Finbridge.TestTask.Infrastructure.Logging;
using Quartz;

namespace Batura.Finbridge.TestTask.Api.Jobs;

/// <summary>
/// Базовый класс для работы с job Quartz
/// </summary>
public abstract class QuartzJobBase : IJob
{
    private readonly string _jobName;

    protected readonly ICorrelationContext _correlationContext;
    protected readonly ILogger? _logger;

    /// <summary>
    /// Создает экземпляр Quartz job
    /// </summary>
    /// <param name="correlationContext">Контекст корреляции, используемый для трассировки выполнения задания</param>
    /// <param name="logger">Необязательный экземпляр логгера</param>
    public QuartzJobBase(ICorrelationContext correlationContext, ILogger? logger = null)
    {
        _correlationContext = correlationContext
            ?? throw new ArgumentNullException(nameof(correlationContext));

        _logger = logger;

        _jobName = GetType().Name;
    }

    /// <inheritdoc />
    public async Task Execute(IJobExecutionContext context)
    {
        using (_correlationContext.SetCorrelationId(Guid.NewGuid()))
        {
            try
            {
                LogJobStarted();

                await Perform(context.CancellationToken);

                LogJobFinished();
            }
            catch (OperationCanceledException ex)
            {
                LogJobCancelled(ex);

                throw;
            }
            catch (Exception ex)
            {
                LogJobError(ex);
            }
        }
    }

    #region Logging methods

    /// <summary>
    /// Логирует начало выполнения задания
    /// </summary>
    protected virtual void LogJobStarted()
    {
        var msg = string.Format(Resources.JobStarted, _jobName);
        _logger?.LogInformation(msg);
    }

    /// <summary>
    /// Логирует успешное завершение задания
    /// </summary>
    protected virtual void LogJobFinished()
    {
        var msg = string.Format(Resources.JobFinished, _jobName);
        _logger?.LogInformation(msg);
    }

    /// <summary>
    /// Логирует отмену выполнения задания
    /// </summary>
    /// <param name="ex">Исключение, возникшее при отмене</param>
    protected virtual void LogJobCancelled(OperationCanceledException ex)
    {
        var msg = string.Format(Resources.JobCancelled, _jobName);
        _logger?.LogInformation(ex, msg);
    }

    /// <summary>
    /// Логирует ошибку выполнения задания
    /// </summary>
    /// <param name="ex">Исключение, возникшее во время выполнения</param>
    protected virtual void LogJobError(Exception ex)
    {
        var errorMsg = string.Format(Resources.JobError, _jobName);
        _logger?.LogCritical(ex, errorMsg);
    }

    #endregion

    /// <summary>
    /// Выполняет основную логику задания
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    protected abstract Task Perform(CancellationToken cancellationToken);
}