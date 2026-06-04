namespace Batura.Finbridge.TestTask.Application.Exceptions;

/// <summary>
/// Ошибка логики выполнения операции
/// </summary>
public sealed class OperationLogicException : Exception
{
    public OperationLogicException() : base() { }

    public OperationLogicException(string message) : base(message) { }

    public OperationLogicException(string format, params object?[] args)
        : base(string.Format(format, args)) { }

    public OperationLogicException(string message, Exception inner)
        : base(message, inner) { }
}