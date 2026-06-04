using Microsoft.EntityFrameworkCore;

namespace Batura.Finbridge.TestTask.Application.Commands;

/// <summary>
/// Команда для работы с Entity Framework
/// </summary>
/// <remarks>
/// Интерфейс определяет команду, которая работает напрямую с контекстом базы данных.
/// Такие команды реализуют паттерн Transaction Script.
/// </remarks>
/// <typeparam name="T">Тип контекста базы данных</typeparam>
public interface ICommand<T> where T : DbContext
{
    /// <summary>
    /// Действия, выполняемые перед выполнением команды
    /// </summary>
    /// <param name="context">Контекст базы данных</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task BeforeExecuteAsync(T context, CancellationToken cancellationToken);

    /// <summary>
    /// Выполняет команду
    /// </summary>
    /// <param name="context">Контекст базы данных</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task ExecuteAsync(T context, CancellationToken cancellationToken);
}