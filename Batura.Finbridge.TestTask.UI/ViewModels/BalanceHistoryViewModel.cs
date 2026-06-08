namespace Batura.Finbridge.TestTask.UI.ViewModels;

/// <summary>
/// Модель представления истории баланса
/// </summary>
public sealed class BalanceHistoryViewModel
{
    /// <summary>
    /// Идентификатор истории баланса
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Полное имя
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Баланс до изменения
    /// </summary>
    public decimal BalanceBefore { get; init; }

    /// <summary>
    /// Баланс после изменения
    /// </summary>
    public decimal BalanceAfter { get; init; }

    /// <summary>
    /// Изменение баланса
    /// </summary>
    public decimal Delta => BalanceAfter - BalanceBefore;

    /// <summary>
    /// Время изменения баланса в UTC
    /// </summary>
    public DateTime ChangedAtUtc { get; init; }
}