namespace Batura.Finbridge.TestTask.Application.Commands.Users;

/// <summary>
/// Результат валидации баланса пользователя
/// </summary>
public enum BalanceValidationResult
{
    /// <summary>
    /// Баланс пользователя корректный
    /// </summary>
    Valid,

    /// <summary>
    /// Отрицательный баланс
    /// </summary>
    NegativeBalance,

    /// <summary>
    /// Баланс превышает лимит
    /// </summary>
    BalanceLimitExceeded
}