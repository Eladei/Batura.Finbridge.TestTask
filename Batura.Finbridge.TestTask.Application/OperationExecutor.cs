using Batura.Finbridge.TestTask.Application.Commands;
using Batura.Finbridge.TestTask.Application.Queries;
using Batura.Finbridge.TestTask.Model;

namespace Batura.Finbridge.TestTask.Application;

/// <summary>
/// Исполнитель операций для Entity Framework
/// </summary>
public class OperationExecutor : IOperationExecutor
{
    private readonly ICommandExecutor<UserDbContext> _commandExecutor;
    private readonly IQueryExecutor<UserDbContext> _queryExecutor;

    /// <summary>
    /// Создает новый экземпляр класса <see cref="OperationExecutor"/>
    /// </summary>
    /// <param name="commandExecutor">Исполнитель команд</param>
    /// <param name="queryExecutor">Исполнитель запросов</param>
    /// <exception cref="ArgumentNullException"></exception>
    public OperationExecutor(
        ICommandExecutor<UserDbContext> commandExecutor,
        IQueryExecutor<UserDbContext> queryExecutor)
    {
        _commandExecutor = commandExecutor
            ?? throw new ArgumentNullException(nameof(commandExecutor));

        _queryExecutor = queryExecutor
            ?? throw new ArgumentNullException(nameof(queryExecutor));
    }

    /// <inheritdoc />
    public Task ExecuteAsync(ICommand<UserDbContext> command, CancellationToken ct)
        => _commandExecutor.ExecuteAsync(command, ct);

    /// <inheritdoc />
    public Task<R> ExecuteAsync<R>(IQuery<UserDbContext, R> query, CancellationToken ct)
        => _queryExecutor.ExecuteAsync(query, ct);
}