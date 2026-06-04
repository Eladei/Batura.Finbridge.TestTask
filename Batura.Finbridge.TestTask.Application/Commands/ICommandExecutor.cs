using Batura.Finbridge.TestTask.Application.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Batura.Finbridge.TestTask.Application.Commands;

/// <summary>
/// Исполнитель команд для работы с Entity Framework
/// </summary>
/// <remarks>
/// Координирует процесс выполнения команд:
/// транзакционное выполнение и логирование.
/// </remarks>
/// <typeparam name="T">Тип контекста базы данных</typeparam>
public interface ICommandExecutor<T> where T : DbContext
{
    /// <summary>
    /// Выполняет команду
    /// </summary>
    /// <param name="command">Команда</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <exception cref="ValidationException"></exception>
    /// <exception cref="OperationLogicException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="Exception"></exception>
    Task ExecuteAsync(ICommand<T> command, CancellationToken cancellationToken);
}