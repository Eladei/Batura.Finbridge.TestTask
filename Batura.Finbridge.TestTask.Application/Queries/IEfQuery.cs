namespace Batura.Finbridge.TestTask.Application.Queries;

/// <summary>
/// Запрос для работы с Entity Framework
/// </summary>
/// <typeparam name="T">Тип контекста базы данных</typeparam>
/// <typeparam name="R">Тип результата</typeparam>
public interface IEfQuery<T, R>
{
    /// <summary>
    /// Выполняет запрос
    /// </summary>
    /// <param name="context">Контекст базы данных</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Результат операции</returns>
    Task<R> ExecuteAsync(T context, CancellationToken cancellationToken);
}