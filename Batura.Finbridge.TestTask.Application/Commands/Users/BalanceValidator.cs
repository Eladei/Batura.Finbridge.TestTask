using Batura.Finbridge.TestTask.Application.Properties;

namespace Batura.Finbridge.TestTask.Application.Commands.Users;

/// <summary>
/// Валидатор баланса пользователя
/// </summary>
public static class BalanceValidator
{
    /// <summary>
    /// Проверяет корректность баланса пользователя
    /// </summary>
    /// <param name="newBalance">Новый баланс</param>
    /// <param name="balanceLimit">Лимит баланса</param>
    /// <returns> баланса пользователя</returns>
    public static BalanceValidationResult Validate(decimal newBalance, decimal balanceLimit)
    {
        if (newBalance < 0)
            return BalanceValidationResult.NegativeBalance;

        if (newBalance > balanceLimit)
            return BalanceValidationResult.BalanceLimitExceeded;

        return BalanceValidationResult.Valid;
    }
}