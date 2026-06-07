namespace Batura.Finbridge.TestTask.Infrastructure.Messaging;

/// <summary>
/// Настройки для Kafka
/// </summary>
public sealed class KafkaOptions
{
    /// <summary>
    /// Адрес Kafka брокера
    /// </summary>
    public string BootstrapServers { get; set; } = null!;

    /// <summary>
    /// Топик для отправки сообщений
    /// </summary>
    public string Topic { get; set; } = null!;
}