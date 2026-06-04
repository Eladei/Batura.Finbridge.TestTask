namespace Batura.Finbridge.TestTask.Api.Contracts;

/// <summary>
/// Информация об изменении баланса пользователя
/// </summary>
public sealed record BalanceHistoryItem
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Полное имя пользователя
    /// </summary>
    public string FullName { get; init; } = string.Empty;

    /// <summary>
    /// Баланс до изменения
    /// </summary>
    public decimal BalanceBefore { get; init; }

    /// <summary>
    /// Баланс после изменения
    /// </summary>
    public decimal BalanceAfter { get; init; }

    /// <summary>
    /// Время изменения баланса
    /// </summary>
    public DateTime ChangedAt { get; init; }
}