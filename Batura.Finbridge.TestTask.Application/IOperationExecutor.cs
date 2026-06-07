using Batura.Finbridge.TestTask.Application.Commands;
using Batura.Finbridge.TestTask.Application.Queries;
using Batura.Finbridge.TestTask.Model;

namespace Batura.Finbridge.TestTask.Application;

/// <summary>
/// Исполнитель операций
/// </summary>
public interface IOperationExecutor : ICommandExecutor<UserDbContext>, IQueryExecutor<UserDbContext> { }