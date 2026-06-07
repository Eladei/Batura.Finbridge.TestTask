namespace Batura.Finbridge.TestTask.Infrastructure.Messaging;

/// <summary>
/// Ошибка публикации события
/// </summary>
public class EventPublishingException : Exception
{
    public EventPublishingException(string message, Exception innerException)
        : base(message, innerException) { }
}