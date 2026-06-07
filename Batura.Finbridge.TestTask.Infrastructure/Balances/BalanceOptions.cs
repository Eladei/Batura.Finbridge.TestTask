namespace Batura.Finbridge.TestTask.Infrastructure.Messaging;

/// <summary>
/// Настройки баланса пользователей
/// </summary>
public sealed class BalanceOptions
{
    /// <summary>
    /// Лимит баланса пользователя
    /// </summary>
    public decimal BalanceLimit { get; set; }
}