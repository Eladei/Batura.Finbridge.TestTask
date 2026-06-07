using Microsoft.EntityFrameworkCore;

namespace Batura.Finbridge.TestTask.Application.Commands;

/// <summary>
/// Базовая команда для работы с Entity Framework
/// </summary>
/// <typeparam name="T">Тип контекста базы данных</typeparam>
/// <remarks>
/// Команда напрямую работает с контекстом базы данных и реализует паттерн Transaction Script
/// </remarks>
public abstract class CommandBase<T> : ICommand<T> where T : DbContext
{
    public virtual Task BeforeExecuteAsync(T context, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public abstract Task ExecuteAsync(T context, CancellationToken cancellationToken = default);
}