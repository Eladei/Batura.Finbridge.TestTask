namespace Batura.Finbridge.TestTask.Application.Exceptions;

/// <summary>
/// Ошибка валидации
/// </summary>
public sealed class ValidationException : Exception
{
    public ValidationException() : base() { }

    public ValidationException(string message) : base(message) { }

    public ValidationException(string format, params object?[] args)
        : base(string.Format(format, args)) { }

    public ValidationException(string message, Exception inner)
        : base(message, inner) { }
}