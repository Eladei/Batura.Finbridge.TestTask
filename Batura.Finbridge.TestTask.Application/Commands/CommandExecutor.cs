using Batura.Finbridge.TestTask.Application.Exceptions;
using Batura.Finbridge.TestTask.Application.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Batura.Finbridge.TestTask.Application.Commands;

/// <summary>
/// Исполнитель команд для работы с Entity Framework
/// </summary>
/// <remarks>
/// Координирует процесс выполнения команд:
/// транзакционное выполнение и логирование.
/// </remarks>
/// <typeparam name="T">Тип контекста базы данных</typeparam>
public class CommandExecutor<T> : ICommandExecutor<T> where T : DbContext
{
    protected readonly IDbContextFactory<T> _contextFactory;
    protected readonly ILogger<CommandExecutor<T>>? _logger;

    /// <summary>
    /// Создаёт экземпляр исполнителя команд EF
    /// </summary>
    /// <param name="contextFactory">Фабрика контекста базы данных</param>
    /// <param name="logger">Опциональный логгер</param>
    public CommandExecutor(
        IDbContextFactory<T> contextFactory,
        ILogger<CommandExecutor<T>>? logger = null)
    {
        _contextFactory = contextFactory
            ?? throw new ArgumentNullException(nameof(contextFactory));

        _logger = logger;
    }

    /// <inheritdoc />
    public virtual async Task ExecuteAsync(ICommand<T> command, CancellationToken cancellationToken)
    {
        var commandName = command.GetType().Name;

        _logger?.LogInformation(Resources.CommandExecutingStarted, commandName);

        using var context = await CreateDbContextAsync(commandName, cancellationToken);

        try
        {
            await command.BeforeExecuteAsync(context, cancellationToken);
        }
        catch (ValidationException ex)
        {
            _logger?.LogWarning(ex, Resources.ValidationError, commandName);

            throw;
        }
        catch (Exception ex)
        {
            _logger?.LogCritical(ex, Resources.CommandExecutingError, commandName);

            throw;
        }

        using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await command.ExecuteAsync(context, cancellationToken);

            await context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            _logger?.LogInformation(Resources.CommandExecutingSuccessfullyFinished, commandName);

            return;
        }
        catch (OperationLogicException ex)
        {
            await transaction.RollbackAsync(cancellationToken);

            _logger?.LogWarning(ex, Resources.OperationLogicError, commandName);

            throw;
        }
        catch (OperationCanceledException ex)
        {
            await transaction.RollbackAsync(cancellationToken);

            _logger?.LogInformation(ex, Resources.CommandExecutingCancelled, commandName);

            throw;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);

            _logger?.LogCritical(ex, Resources.CommandExecutingError, commandName);

            throw;
        }
    }

    /// <summary>
    /// Создаёт контекст базы данных
    /// </summary>
    /// <param name="commandName">Имя команды</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Экземпляр контекста базы данных</returns>
    /// <exception cref="InvalidOperationException"></exception>
    protected virtual async Task<T> CreateDbContextAsync(string commandName, CancellationToken cancellationToken)
    {
        T context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        if (context is null)
        {
            var invalidOperEx = new InvalidOperationException(Resources.CantCreateDbContext);

            _logger?.LogCritical(invalidOperEx, Resources.CommandExecutingError, commandName);

            throw invalidOperEx;
        }

        return context;
    }
}