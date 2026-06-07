using Batura.Finbridge.TestTask.Application.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Batura.Finbridge.TestTask.Application.Queries;

/// <summary>
/// Исполнитель запросов для работы с Entity Framework
/// </summary>
/// <typeparam name="T">Тип контекста базы данных</typeparam>
public class EfQueryExecutor<T> : IEfQueryExecutor<T> where T : DbContext
{
    protected readonly IDbContextFactory<T> _contextFactory;
    protected readonly ILogger? _logger;

    /// <summary>
    /// Создаёт экземпляр исполнителя запросов EF
    /// </summary>
    /// <param name="contextFactory">Фабрика контекста базы данных</param>
    /// <param name="logger">Опциональный логгер</param>
    /// <exception cref="ArgumentNullException"></exception>
    public EfQueryExecutor(IDbContextFactory<T> contextFactory, ILogger? logger = null)
    {
        _contextFactory = contextFactory
            ?? throw new ArgumentNullException(nameof(contextFactory));

        _logger = logger;
    }

    /// <inheritdoc />
    public virtual async Task<R> ExecuteAsync<R>(IEfQuery<T, R> query, CancellationToken cancellationToken)
    {
        var queryName = query.GetType().Name;

        _logger?.LogInformation(Resources.QueryExecutingStarted, queryName);

        using var dbContext = await CreateDbContextAsync(queryName, cancellationToken);

        try
        {
            var result = await query.ExecuteAsync(dbContext, cancellationToken);

            _logger?.LogInformation(Resources.QueryExecutingSuccessfullyFinished, queryName);

            return result;
        }
        catch (OperationCanceledException ex)
        {
            _logger?.LogInformation(ex, Resources.QueryExecutingCancelled, queryName);

            throw;
        }
        catch (Exception ex)
        {
            _logger?.LogCritical(ex, Resources.QueryExecutingError, queryName);

            throw;
        }
    }

    /// <summary>
    /// Создаёт контекст базы данных
    /// </summary>
    /// <param name="queryName">Имя запроса</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Экземпляр контекста базы данных</returns>
    /// <exception cref="InvalidOperationException"></exception>
    protected virtual async Task<T> CreateDbContextAsync(string queryName, CancellationToken cancellationToken)
    {
        T context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        if (context is null)
        {
            var invalidOperEx = new InvalidOperationException(Resources.CantCreateDbContext);

            _logger?.LogCritical(invalidOperEx, Resources.QueryExecutingError, queryName);

            throw invalidOperEx;
        }

        return context;
    }
}