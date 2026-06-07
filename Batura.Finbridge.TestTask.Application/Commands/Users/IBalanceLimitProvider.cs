namespace Batura.Finbridge.TestTask.Application.Commands.Users;

/// <summary>
/// Провайдер лимита баланса пользователя
/// </summary>
public interface IBalanceLimitProvider
{
    /// <summary>
    /// Получает лимит баланса пользователя
    /// </summary>
    /// <returns>Лимит баланса</returns>
    decimal GetBalanceLimit();
}