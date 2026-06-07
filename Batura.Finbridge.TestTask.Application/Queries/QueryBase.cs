using Microsoft.EntityFrameworkCore;

namespace Batura.Finbridge.TestTask.Application.Queries;

/// <summary>
/// Базовый запрос для работы с Entity Framework
/// </summary>
/// <typeparam name="T">Тип контекста базы данных</typeparam>
/// <typeparam name="R">Тип результата</typeparam>
public abstract class QueryBase<T, R> : IQuery<T, R> where T : DbContext
{
    /// <inheritdoc />
    public abstract Task<R> ExecuteAsync(T context, CancellationToken cancellationToken = default);
}