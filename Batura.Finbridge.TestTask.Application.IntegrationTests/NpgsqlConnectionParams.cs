namespace Batura.Finbridge.TestTask.Application.IntegrationTests;

/// <summary>
/// Параметры подключения к PostgreSQL серверу
/// </summary>
public record NpgsqlConnectionParams
{
    /// <summary>
    /// Строка подключения
    /// </summary>
    public string ConnectionString { get; init; } = null!;
}