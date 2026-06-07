using Microsoft.EntityFrameworkCore;

namespace Batura.Finbridge.TestTask.Application.Queries;

/// <summary>
/// Исполнитель запросов для работы с Entity Framework
/// </summary>
/// <typeparam name="T">Тип контекста базы данных</typeparam>
public interface IEfQueryExecutor<T> where T : DbContext
{
    /// <summary>
    /// Выполняет запрос
    /// </summary>
    /// <typeparam name="R">Тип результата</typeparam>
    /// <param name="query">Запрос</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат запроса</returns>
    Task<R> ExecuteAsync<R>(IEfQuery<T, R> query, CancellationToken cancellationToken);
}